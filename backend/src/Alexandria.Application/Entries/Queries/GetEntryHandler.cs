using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Entries.Responses;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Entries.Queries;

public record GetEntryQuery(Guid EntryId) : IRequest<ErrorOr<GetEntryResponse>>;
public record GetEntryResponse(EntryResponse Entry);

public class GetEntryHandler : IRequestHandler<GetEntryQuery, ErrorOr<GetEntryResponse>>
{
    public readonly ILogger<GetEntryHandler> _logger;
    public readonly IEntryRepository _entryRepository;

    public GetEntryHandler(ILogger<GetEntryHandler> logger, IEntryRepository entryRepository)
    {
        _logger = logger;
        _entryRepository = entryRepository;
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

        var entryResponse = EntryResponse.FromEntry(entry);
        return new GetEntryResponse(entryResponse);
    }
}