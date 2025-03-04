using Alexandria.Application.Common;
using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Common.Pagination;
using Alexandria.Application.Entries.Responses;
using Alexandria.Application.Tags.Responses;
using Alexandria.Application.Users.Responses;
using Alexandria.Domain.Common.Entities.Tag;
using Alexandria.Domain.EntryAggregate;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Entries.Queries;

[Flags]
public enum GetEntriesOptions
{
    None = 0,
    IncludeThumbnails = 1 << 0, // TODO: Implement thumbnails
    IncludeTags = 1 << 1,
    IncludeDocument = 1 << 2
}

public record GetEntriesQuery(
    PaginatedRequest PaginatedParams,
    GetEntriesOptions Options = GetEntriesOptions.None,
    Guid? TagId = null) : IRequest<ErrorOr<GetEntriesResponse>>;
public record GetEntriesResponse(PaginatedResponse<EntryResponse> Entries);
public class GetEntriesHandler : IRequestHandler<GetEntriesQuery, ErrorOr<GetEntriesResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<GetEntriesHandler> _logger;
    private readonly ITaggingService _taggingService;

    private const int MAX_PAGE_SIZE = 100;

    public GetEntriesHandler(
        IAppDbContext context,
        ILogger<GetEntriesHandler> logger,
        ITaggingService taggingService)
    {
        _context = context;
        _logger = logger;
        _taggingService = taggingService;
    }

    public async Task<ErrorOr<GetEntriesResponse>> Handle(GetEntriesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetEntriesHandler processing request...");
        if (request.PaginatedParams.PageSize > MAX_PAGE_SIZE)
        {
            _logger.LogInformation(
                "User attempted to retrieve more than {PageSize} entries (exceeds maximum of {MaxPageSize}).",
                request.PaginatedParams.PageSize,
                MAX_PAGE_SIZE);
            return ApplicationErrors.BadQueryError;
        }
        if (request.Options.HasFlag(GetEntriesOptions.IncludeThumbnails))
        {
            _logger.LogInformation("IncludeThumbnails enabled");
            throw new NotImplementedException("Thumbnails are not implemented");
        }
        
        Entry? cursorEntry;
        var query = _context.Entries.AsQueryable();

        // Only retrieve entries with tag, if one specified
        if (request.TagId != null)
        {
            _logger.LogInformation("TagId set to: {TagId}", request.TagId);
            
            var tag = await _context.Tags.FindAsync([request.TagId], cancellationToken);
            if (tag == null)
            {
                _logger.LogInformation("Tag not found with ID {TagID}", request.TagId);
                return TagErrors.TagNotFound;
            }
            var entryIds = await _taggingService.GetEntityIdsWithTag<Entry>(tag, cancellationToken);
            query = query.Where(e => entryIds.Contains(e.Id));
        }
        
        if (request.PaginatedParams.CursorId != null)
        {
            _logger.LogInformation("CursorId set to: {CursorId}", request.PaginatedParams.CursorId);
            
            cursorEntry = await _context.Entries.FindAsync([request.PaginatedParams.CursorId], cancellationToken);
            if (cursorEntry == null)
            {
                _logger.LogInformation("Entry with ID \'{CursorId}\' not found", request.PaginatedParams.CursorId);
                return EntryErrors.NotFound;
            }
            
            query = query
                .Where(entry => entry.CreatedAtUtc < cursorEntry.CreatedAtUtc ||
                                (entry.CreatedAtUtc == cursorEntry.CreatedAtUtc && entry.Id < cursorEntry.Id));
        }

        if (request.Options.HasFlag(GetEntriesOptions.IncludeDocument))
        {
            query = query.Include(entry => entry.Document);
        }
        
        // Replace the existing query execution and nextCursor block with:
        var entries = await query
            .OrderByDescending(entry => entry.CreatedAtUtc)
            .ThenByDescending(entry => entry.Id)
            .Take(request.PaginatedParams.PageSize + 1) // Retrieve extra record to check if next page exists
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {EntryCount} entries", entries.Count);

        Guid? nextCursor = null;
        if (entries.Count > request.PaginatedParams.PageSize)
        {
            nextCursor = entries[^2].Id;
            entries = entries.Take(request.PaginatedParams.PageSize).ToList();
            _logger.LogInformation("NextCursor set to {CursorId}", nextCursor);
        }
        
        var entryResponses = await GetEntryResponses(entries, request.Options, cancellationToken);
        
        var pagedResponse = new PaginatedResponse<EntryResponse>()
        {
            Data = entryResponses.ToList(),
            Paging = new PagingData { NextCursor = nextCursor }
        };
        
        return new GetEntriesResponse(pagedResponse);
    }

    private async Task<IEnumerable<EntryResponse>> GetEntryResponses(
        List<Entry> entries,
        GetEntriesOptions options,
        CancellationToken cancellationToken)
    {
        if (entries.Count == 0) return [];
        
        var createdByUserIds = entries
            .Select(x => x.CreatedById)
            .Distinct();

        var users = await _context.Users
            .Where(x => createdByUserIds.Contains(x.Id))
            .ToDictionaryAsync(
                user => user.Id,
                user => user,
                cancellationToken);
        
        // Retrieve and append tags if option was enabled
        IReadOnlyDictionary<Guid, IEnumerable<Tag>>? entryTags = null;
        if (options.HasFlag(GetEntriesOptions.IncludeTags))
        { 
            entryTags = await _taggingService.GetEntitiesTags(entries, cancellationToken);
        }
        
        var entryResponses = entries
            .Select(entry => new EntryResponse
            {
                Id = entry.Id,
                Name = entry.Name,
                Tags = GetTagResponse(entry.Id),
                Description = entry.Description,
                Document = !options.HasFlag(GetEntriesOptions.IncludeDocument) ? null : new DocumentResponse
                {
                    Id = entry.Document!.Id,
                    Name = entry.Document!.Name,
                    ImagePath = entry.Document!.ImagePath,
                    FileExtension = entry.Document!.FileExtension
                },
                CreatedBy = GetUserResponse(entry.CreatedById),
                CreatedAtUtc = entry.CreatedAtUtc,
                DeletedAtUtc = entry.DeletedAtUtc
            })
            .ToList();
        
        return entryResponses;

        UserResponse? GetUserResponse(Guid userId) =>
            users.TryGetValue(userId, out var user)
                ? new UserResponse { Id = user.Id, Name = user.Name }
                : null;
        
        IReadOnlyList<TagResponse>? GetTagResponse(Guid entryId)
        {
            if (!options.HasFlag(GetEntriesOptions.IncludeTags))
            {
                return null;
            }
            
            return entryTags!.TryGetValue(entryId, out var tags)
                ? tags.Select(tag => new TagResponse { Id = tag.Id, Name = tag.Name }).ToList()
                : [];
        }
    }
}