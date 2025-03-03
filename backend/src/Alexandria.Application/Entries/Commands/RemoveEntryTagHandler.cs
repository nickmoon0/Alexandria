using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common.Entities.Tag;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Entries.Commands;

public record RemoveEntryTagCommand(Guid EntryId, Guid TagId) : IRequest<ErrorOr<Success>>;

public class RemoveEntryTagHandler : IRequestHandler<RemoveEntryTagCommand, ErrorOr<Success>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<RemoveEntryTagHandler> _logger;
    private readonly ITaggingService _taggingService;

    public RemoveEntryTagHandler(IAppDbContext context, ILogger<RemoveEntryTagHandler> logger, ITaggingService taggingService)
    {
        _context = context;
        _logger = logger;
        _taggingService = taggingService;
    }

    public async Task<ErrorOr<Success>> Handle(RemoveEntryTagCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.Entries.FindAsync([request.EntryId], cancellationToken);
        if (entry == null)
        {
            _logger.LogInformation("Could not find entry with ID {ID}", request.EntryId);
            return EntryErrors.NotFound;
        }
        
        var tag = await _context.Tags.FindAsync([request.TagId], cancellationToken);
        if (tag == null)
        {
            _logger.LogInformation("Could not find tag with ID {ID}", request.TagId);
            return TagErrors.TagNotFound;
        }
        
        var removeTagResult = await _taggingService.RemoveTag(entry, tag);
        if (removeTagResult.IsError)
        {
            _logger.LogError("Failed to remove tag with ID {TagID} from entry with Entry ID {EntryID}",
                request.TagId, request.EntryId);
            return removeTagResult.Errors;
        }
        
        return Result.Success;
    }
}