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
    GetEntriesOptions Options = GetEntriesOptions.None) : IRequest<ErrorOr<GetEntriesResponse>>;
public record GetEntriesResponse(PaginationResponse<EntryResponse> Entries);
public class GetEntriesHandler : IRequestHandler<GetEntriesQuery, ErrorOr<GetEntriesResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<GetEntriesHandler> _logger;
    private readonly ITaggingService _taggingService;

    private static readonly int _maxPageSize = 100;
    
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
        if (request.PaginatedParams.PageSize > 100)
        {
            _logger.LogInformation(
                "User attempted to retrieve more than {PageSize} entries (exceeds maximum of {MaxPageSize}).",
                request.PaginatedParams.PageSize,
                _maxPageSize);
            return ApplicationErrors.BadQueryError;
        }
        if (request.Options.HasFlag(GetEntriesOptions.IncludeThumbnails))
        {
            _logger.LogInformation("IncludeThumbnails enabled");
            throw new NotImplementedException("Thumbnails are not implemented");
        }
        
        Entry? cursorEntry;
        IQueryable<Entry> entriesQuery = _context.Entries;
        
        if (request.PaginatedParams.CursorId != null)
        {
            _logger.LogInformation("CursorId set to: {CursorId}", request.PaginatedParams.CursorId);
            
            cursorEntry = await _context.Entries.FindAsync([request.PaginatedParams.CursorId], cancellationToken);
            if (cursorEntry == null)
            {
                _logger.LogInformation("Entry with ID \'{CursorId}\' not found", request.PaginatedParams.CursorId);
                return EntryErrors.NotFound;
            }
            
            entriesQuery = entriesQuery
                .Where(entry => entry.CreatedAtUtc < cursorEntry.CreatedAtUtc ||
                                (entry.CreatedAtUtc == cursorEntry.CreatedAtUtc && entry.Id < cursorEntry.Id));
        }

        if (request.Options.HasFlag(GetEntriesOptions.IncludeDocument))
        {
            entriesQuery = entriesQuery.Include(entry => entry.Document);
        }
        
        var entries = await entriesQuery
            .OrderByDescending(entry => entry.CreatedAtUtc)
            .ThenByDescending(entry => entry.Id)
            .Take(request.PaginatedParams.PageSize)
            .ToListAsync(cancellationToken);
        
        _logger.LogInformation("Retrieved {EntryCount} entries", entries.Count);
        
        var entryResponses = await GetEntryResponses(entries, request.Options, cancellationToken);

        var lastEntry = entries.Last();

        Guid? nextCursor = null;
        if (entries.Count != 0)
        {
            var hasNextPage = await _context.Entries.AnyAsync(entry =>
                    entry.CreatedAtUtc < lastEntry.CreatedAtUtc ||
                    (entry.CreatedAtUtc == lastEntry.CreatedAtUtc && entry.Id < lastEntry.Id),
                cancellationToken);
            if (hasNextPage)
            {
                nextCursor = lastEntry.Id;
                _logger.LogInformation("NextCursor set to {CursorId}", nextCursor);
            }
        }
        
        var pagedResponse = new PaginationResponse<EntryResponse>()
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