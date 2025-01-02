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
    IncludeTags = 1 << 1
}

public record GetEntriesQuery(
    PaginatedRequest PaginatedParams,
    GetEntriesOptions Options = GetEntriesOptions.None) : IRequest<ErrorOr<GetEntriesResponse>>;
public record GetEntriesResponse(IEnumerable<EntryResponse> Entries);

public class GetEntriesHandler : IRequestHandler<GetEntriesQuery, ErrorOr<GetEntriesResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<GetEntriesHandler> _logger;
    private readonly ITaggingService _taggingService;
    
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
        if (request.Options.HasFlag(GetEntriesOptions.IncludeThumbnails))
        {
            throw new NotImplementedException("Thumbnails are not implemented");
        }
        
        Entry? cursorEntry = null;
        if (request.PaginatedParams.CursorId != null)
        {
            cursorEntry = await _context.Entries.FindAsync([request.PaginatedParams.CursorId], cancellationToken);
            if (cursorEntry == null) return EntryErrors.NotFound;
        }

        IQueryable<Entry> entriesQuery = _context.Entries;

        if (cursorEntry != null && request.PaginatedParams is { Direction: PaginationDirection.NextPage })
        {
            entriesQuery = entriesQuery
                .Where(entry => entry.CreatedAtUtc < cursorEntry.CreatedAtUtc ||
                                (entry.CreatedAtUtc == cursorEntry.CreatedAtUtc && entry.Id < cursorEntry.Id))
                .OrderByDescending(entry => entry.CreatedAtUtc)
                .ThenByDescending(entry => entry.Id);
        }
        else if (cursorEntry != null && request.PaginatedParams is { Direction: PaginationDirection.PreviousPage })
        {
            entriesQuery = entriesQuery
                .Where(entry => entry.CreatedAtUtc > cursorEntry.CreatedAtUtc ||
                                (entry.CreatedAtUtc == cursorEntry.CreatedAtUtc && entry.Id > cursorEntry.Id))
                .OrderBy(entry => entry.CreatedAtUtc)
                .ThenBy(entry => entry.Id);
        }
        else
        {
            entriesQuery = entriesQuery
                .OrderByDescending(entry => entry.CreatedAtUtc)
                .ThenByDescending(entry => entry.Id);
        }
        
        var entries = await entriesQuery
            .Take(request.PaginatedParams.PageSize)
            .ToListAsync(cancellationToken);

        if (request.PaginatedParams is { Direction: PaginationDirection.PreviousPage })
        {
            entries.Reverse();
        }
        
        var entryResponses = await GetEntryResponses(entries, request.Options, cancellationToken);
        return new GetEntriesResponse(entryResponses);
    }

    private async Task<IEnumerable<EntryResponse>> GetEntryResponses(
        List<Entry> entries,
        GetEntriesOptions options,
        CancellationToken cancellationToken)
    {
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