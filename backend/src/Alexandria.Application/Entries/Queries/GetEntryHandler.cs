using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Common.Responses;
using Alexandria.Application.Entries.Responses;
using Alexandria.Application.Users.Responses;
using Alexandria.Domain.Common.Entities.Tag;
using Alexandria.Domain.EntryAggregate;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Entries.Queries;

public record GetEntryQuery(Guid EntryId) : IRequest<ErrorOr<GetEntryResponse>>;
public record GetEntryResponse(EntryResponse Entry);

public class GetEntryHandler : IRequestHandler<GetEntryQuery, ErrorOr<GetEntryResponse>>
{
    private readonly ILogger<GetEntryHandler> _logger;
    private readonly IEntryRepository _entryRepository;
    private readonly ITaggingService _taggingService;
    private readonly IUserRepository _userRepository;
    
    public GetEntryHandler(
        ILogger<GetEntryHandler> logger,
        IEntryRepository entryRepository,
        ITagRepository tagRepository,
        ITaggingService taggingService,
        IUserRepository userRepository)
    {
        _logger = logger;
        _entryRepository = entryRepository;
        _taggingService = taggingService;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<GetEntryResponse>> Handle(GetEntryQuery request, CancellationToken cancellationToken)
    {
        // Get the entry from provided ID
        var entryResult = await _entryRepository.FindByIdAsync(
            request.EntryId, cancellationToken, [FindOptions.IncludeDocument, FindOptions.IncludeComments]);
        
        if (entryResult.IsError)
        {
            _logger.LogError("Get entry with Id: {EntryId} failed.", request.EntryId);
            return entryResult.Errors;
        }
        
        var entry = entryResult.Value;

        // Get the CreatedBy user response object
        var createdByUserResult = await _userRepository.FindByIdAsync(entry.CreatedById, cancellationToken);
        if (createdByUserResult.IsError)
        {
            _logger.LogError("Failed to retrieve user with ID {ID}", entry.CreatedById);
            return createdByUserResult.Errors;
        }
        var createdByUser = createdByUserResult.Value;
        var userResponse = new UserResponse
        {
            Id = createdByUser.Id,
            FirstName = createdByUser.Name.FirstName,
            LastName = createdByUser.Name.LastName
        };
        
        // Create comment response objects
        var commentResponsesResult = await GetCommentResponses(entry, cancellationToken);
        if (commentResponsesResult.IsError)
        {
            return commentResponsesResult.Errors;
        }
        
        // Create document response object
        var documentResponseResult = await GetDocumentResponse(entry, cancellationToken);
        if (documentResponseResult.IsError)
        {
            return documentResponseResult.Errors;
        }
        
        // Get the tag response objects
        var tagResponsesResult = await GetTagResponses(entry, cancellationToken);
        if (tagResponsesResult.IsError)
        {
            return tagResponsesResult.Errors;
        }
        
        var entryResponse = new EntryResponse
        {
            Id = entry.Id,
            Name = entry.Name,
            Description = entry.Description,
            Document = documentResponseResult.Value,
            Comments = commentResponsesResult.Value.ToList(),
            CreatedBy = userResponse,
            CreatedAtUtc = entry.CreatedAtUtc,
            DeletedAtUtc = entry.DeletedAtUtc,
            Tags = tagResponsesResult.Value.ToList(),
        };

        return new GetEntryResponse(entryResponse);
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
            return DocumentErrors.DocumentNull;
        }
        var document = entry.Document;
        
        var createdByResult = await _userRepository.FindByIdAsync(document.CreatedById, cancellationToken);
        if (createdByResult.IsError)
        {
            _logger.LogError("Failed to retrieve user with ID {ID} when getting Document CreatedBy user", document.CreatedById);
            return createdByResult.Errors;
        }
        var createdByUser = createdByResult.Value;
        var userResponse = new UserResponse
        {
            Id = createdByUser.Id,
            FirstName = createdByUser.Name.FirstName,
            LastName = createdByUser.Name.LastName,
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
        var users = await _userRepository.FindByIdsAsync(commentUserIds, cancellationToken);
        if (users.IsError)
        {
            _logger.LogError("Failed to retrieve users for comments in entry with ID {ID}", entry.Id);
            return users.Errors;
        }
        
        var usersDict = users.Value
            .Select(user => new UserResponse
            {
                Id = user.Id,
                FirstName = user.Name.FirstName,
                LastName = user.Name.LastName,
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