using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common.Entities.Tag;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Entries.Commands;

public record TagEntryCommand(Guid EntryId, Guid TagId) : IRequest<ErrorOr<Success>>;

public class TagEntryHandler : IRequestHandler<TagEntryCommand, ErrorOr<Success>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<TagEntryHandler> _logger;
    private readonly ITaggingService _taggingService;
    
    public TagEntryHandler(
        IAppDbContext context,
        ILogger<TagEntryHandler> logger,
        ITaggingService taggingService)
    {
        _context = context;
        _logger = logger;
        _taggingService = taggingService;
    }

    public async Task<ErrorOr<Success>> Handle(TagEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.Entries.FindAsync([request.EntryId], cancellationToken);
        if (entry == null)
        {
            _logger.LogError("Entry not found with ID {ID}", request.EntryId);
            return EntryErrors.NotFound;
        }
        
        var tag = await _context.Tags.FindAsync([request.TagId], cancellationToken);
        if (tag == null)
        {
            _logger.LogError("Tag not found with ID {ID}", request.TagId);
            return TagErrors.TagNotFound;
        }

        var taggingResult = await _taggingService.TagEntity(entry, tag);
        if (taggingResult.IsError)
        {
            _logger.LogError("Tagging not successful with Entry ID {EntryId} and Tag ID {TagId}", 
                request.EntryId,
                request.TagId);
            return taggingResult.Errors;
        }
        
        return Result.Success;
    }
}