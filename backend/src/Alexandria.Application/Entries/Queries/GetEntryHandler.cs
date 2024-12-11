using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Common.Responses;
using Alexandria.Application.Entries.Responses;
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
    private readonly ITagRepository _tagRepository;

    public GetEntryHandler(
        ILogger<GetEntryHandler> logger,
        IEntryRepository entryRepository, ITagRepository tagRepository)
    {
        _logger = logger;
        _entryRepository = entryRepository;
        _tagRepository = tagRepository;
    }

    public async Task<ErrorOr<GetEntryResponse>> Handle(GetEntryQuery request, CancellationToken cancellationToken)
    {
        var entryResult = await _entryRepository.FindByIdAsync(
            request.EntryId, cancellationToken, [FindOptions.IncludeDocument, FindOptions.IncludeComments]);
        
        if (entryResult.IsError)
        {
            _logger.LogError("Get entry with Id: {EntryId} failed.", request.EntryId);
            return entryResult.Errors;
        }
        
        var entry = entryResult.Value;

        var tags = await _tagRepository.GetEntityTags(entry, cancellationToken);
        var entryResponse = EntryResponse.FromEntry(entry);
        
        // Append tags to response
        entryResponse.Tags = tags.Value
            .Select(TagResponse.FromTag)
            .ToList();
        
        return new GetEntryResponse(entryResponse);
    }
}