using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Tags.Responses;
using Alexandria.Application.Entries.Responses;
using Alexandria.Application.Users.Responses;
using Alexandria.Domain.EntryAggregate;
using Alexandria.Domain.EntryAggregate.Errors;
using Alexandria.Domain.UserAggregate;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Entries.Queries;

[Flags]
public enum GetEntryOptions
{
    None = 0,
    IncludeComments = 1,
    IncludeDocument = 2,
    IncludeTags = 4
}

public record GetEntryQuery(Guid EntryId, GetEntryOptions Options) : IRequest<ErrorOr<GetEntryResponse>>;
public record GetEntryResponse(EntryResponse Entry);

public class GetEntryHandler : IRequestHandler<GetEntryQuery, ErrorOr<GetEntryResponse>>
{
    private readonly ILogger<GetEntryHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly ITaggingService _taggingService;
    
    public GetEntryHandler(
        ILogger<GetEntryHandler> logger,
        IAppDbContext context,
        ITaggingService taggingService)
    {
        _logger = logger;
        _context = context;
        _taggingService = taggingService;
    }

    public async Task<ErrorOr<GetEntryResponse>> Handle(GetEntryQuery request, CancellationToken cancellationToken)
    {
        // Get the entry from provided ID
        var entryResult = await GetEntry(request.EntryId, request.Options, cancellationToken);
        if (entryResult.IsError)
        {
            _logger.LogError("Get entry with Id: {EntryId} failed.", request.EntryId);
            return entryResult.Errors;
        }
        
        var entry = entryResult.Value;

        // Get the CreatedBy user response object
        var createdByUser = await _context.Users.FindAsync([entry.CreatedById], cancellationToken);
        if (createdByUser == null)
        {
            _logger.LogError("Failed to retrieve user with ID {ID}", entry.CreatedById);
            return UserErrors.NotFound;
        }
        
        var userResponse = new UserResponse
        {
            Id = createdByUser.Id,
            Name = createdByUser.Name,
        };
        
        // Create comment response objects
        List<CommentResponse>? comments = null;
        if (request.Options.HasFlag(GetEntryOptions.IncludeComments))
        {
            var commentResponsesResult = await GetCommentResponses(entry, cancellationToken);
            if (commentResponsesResult.IsError)
            {
                return commentResponsesResult.Errors;
            }
            
            comments = commentResponsesResult.Value.ToList();
        }

        
        // Create document response object
        DocumentResponse? document = null;
        if (request.Options.HasFlag(GetEntryOptions.IncludeDocument))
        {
            var documentResponseResult = await GetDocumentResponse(entry, cancellationToken);
            if (documentResponseResult.IsError)
            {
                return documentResponseResult.Errors;
            }
            
            document = documentResponseResult.Value;
        }
        
        // Get the tag response objects
        List<TagResponse>? tags = null;
        if (request.Options.HasFlag(GetEntryOptions.IncludeTags))
        {
            var tagResponsesResult = await GetTagResponses(entry, cancellationToken);
            if (tagResponsesResult.IsError)
            {
                return tagResponsesResult.Errors;
            }
            
            tags = tagResponsesResult.Value.ToList();
        }
        
        var entryResponse = new EntryResponse
        {
            Id = entry.Id,
            Name = entry.Name,
            Description = entry.Description,
            Document = document,
            Comments = comments,
            CreatedBy = userResponse,
            CreatedAtUtc = entry.CreatedAtUtc,
            DeletedAtUtc = entry.DeletedAtUtc,
            Tags = tags,
        };

        return new GetEntryResponse(entryResponse);
    }

    private async Task<ErrorOr<Entry>> GetEntry(
        Guid entryId,
        GetEntryOptions options,
        CancellationToken cancellationToken)
    {
        IQueryable<Entry> query = _context.Entries;

        if (options.HasFlag(GetEntryOptions.IncludeComments))
        {
            query = query.Include(entry => entry.Comments);
        }

        if (options.HasFlag(GetEntryOptions.IncludeDocument))
        {
            query = query.Include(entry => entry.Document);
        }
        
        var entry = await query.SingleOrDefaultAsync(entry => entry.Id == entryId, cancellationToken);
        if (entry == null)
        {
            _logger.LogError("Entry not found with ID {ID}", entryId);
            return Error.NotFound();
        }

        return entry;
    }
    
    private async Task<ErrorOr<IEnumerable<TagResponse>>> GetTagResponses(Entry entry, CancellationToken cancellationToken)
    {
        var tags = await _taggingService.GetEntityTags(entry, cancellationToken);
        if (tags.IsError)
        {
            _logger.LogError("Failed to get tags for entity with type {EntityType} and ID {EntityId}",
                nameof(Entry),
                entry.Id);
            return tags.Errors;
        }

        var tagResponses = tags.Value.Select(tag => new TagResponse
        {
            Id = tag.Id,
            Name = tag.Name
        }).ToList();
        
        return tagResponses;
    }
    
    private async Task<ErrorOr<DocumentResponse>> GetDocumentResponse(Entry entry, CancellationToken cancellationToken)
    {
        if (entry.Document == null)
        {
            _logger.LogError("Document null for Entry with ID {ID}", entry.Id);
            return EntryErrors.DocumentNull;
        }
        var document = entry.Document;
        
        var createdByUser = await _context.Users.FindAsync([document.CreatedById], cancellationToken);
        if (createdByUser == null)
        {
            _logger.LogError("Failed to retrieve user with ID {ID} when getting Document CreatedBy user", document.CreatedById);
            return UserErrors.NotFound;
        }

        var userResponse = new UserResponse
        {
            Id = createdByUser.Id,
            Name = createdByUser.Name,
        };

        var documentResponse = new DocumentResponse
        {
            Id = document.Id,
            EntryId = document.EntryId,
            Name = document.Name,
            ImagePath = document.ImagePath,
            FileExtension = document.FileExtension,
            CreatedByUser = userResponse,
            CreatedAtUtc = document.CreatedAtUtc,
            DeletedAtUtc = document.DeletedAtUtc,
        };
        return documentResponse;
    }
    
    private async Task<ErrorOr<IEnumerable<CommentResponse>>> GetCommentResponses(Entry entry,
        CancellationToken cancellationToken)
    {
        var commentUserIds = entry.Comments.Select(c => c.CreatedById).ToList();
        var users = await _context.Users
            .Where(user => commentUserIds.Contains(user.Id))
            .ToListAsync(cancellationToken);
        
        var usersDict = users
            .Select(user => new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
            })
            .ToDictionary(x => (Guid)x.Id!, x => x);
        
        var commentResponses = entry.Comments
            .Select(comment => new CommentResponse 
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedBy = usersDict[comment.CreatedById],
                CreatedAtUtc = comment.CreatedAtUtc,
                DeletedAtUtc = comment.DeletedAtUtc,
            })
            .ToList();

        return commentResponses;
    }
}