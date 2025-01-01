using Alexandria.Application.Common;
using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Entries.Responses;
using Alexandria.Application.Tags.Responses;
using Alexandria.Application.Users.Responses;
using Alexandria.Domain.Common.Entities.Tag;
using Alexandria.Domain.EntryAggregate;
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
    Guid? LastEntryIdPaged = null,
    DateTime? LastEntryDatePaged = null,
    int Count = 30,
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
            throw new NotImplementedException("Thumbnails are not yet supported.");
        }
        
        if (request is 
            { LastEntryDatePaged: not null, LastEntryIdPaged: null } or
            { LastEntryIdPaged: null, LastEntryDatePaged: not null })
        {
            _logger.LogInformation("PreviousEntryDatePaged and PreviousEntryIdPaged must both have values or both be null");
            return ApplicationErrors.BadQueryError;
        }
        
        IQueryable<Entry> entriesQuery = _context.Entries
            .OrderByDescending(x => x.CreatedAtUtc)
            .ThenBy(x => x.Id);

        if (request.LastEntryDatePaged != null)
        {
            entriesQuery = entriesQuery.Where(entry => 
                entry.CreatedAtUtc < request.LastEntryDatePaged ||
                (entry.CreatedAtUtc == request.LastEntryDatePaged && entry.Id > request.LastEntryIdPaged));
        }
        
        var entries = await entriesQuery
            .Take(request.Count)
            .ToListAsync(cancellationToken);

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