using Alexandria.Application.Common.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Entries.Commands;

public record TagEntryCommand(Guid EntryId, Guid TagId) : IRequest<ErrorOr<Success>>;

public class TagEntryHandler : IRequestHandler<TagEntryCommand, ErrorOr<Success>>
{
    private readonly ILogger<TagEntryHandler> _logger;
    private readonly IEntryRepository _entryRepository;
    private readonly ITagRepository _tagRepository;

    public TagEntryHandler(
        ILogger<TagEntryHandler> logger,
        IEntryRepository entryRepository,
        ITagRepository tagRepository)
    {
        _logger = logger;
        _entryRepository = entryRepository;
        _tagRepository = tagRepository;
    }

    public async Task<ErrorOr<Success>> Handle(TagEntryCommand request, CancellationToken cancellationToken)
    {
        var entryResult = await _entryRepository.FindByIdAsync(request.EntryId, cancellationToken);
        if (entryResult.IsError)
        {
            _logger.LogError("Entry not found with ID {ID}", request.EntryId);
            return entryResult.Errors;
        }
        var entry = entryResult.Value;
        
        var tagResult = await _tagRepository.FindByIdAsync(request.TagId, cancellationToken);
        if (tagResult.IsError)
        {
            _logger.LogError("Tag not found with ID {ID}", request.TagId);
            return tagResult.Errors;
        }
        var tag = tagResult.Value;

        var taggingResult = await _tagRepository.AddEntityTag(entry, tag, cancellationToken);
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