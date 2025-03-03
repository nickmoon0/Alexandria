using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.Common.Entities.Tag;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Characters.Commands;

public record TagCharacterCommand(Guid CharacterId, Guid TagId) : IRequest<ErrorOr<Success>>;

public class TagCharacterHandler : IRequestHandler<TagCharacterCommand, ErrorOr<Success>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<TagCharacterHandler> _logger;
    private readonly ITaggingService _taggingService;
    
    public TagCharacterHandler(IAppDbContext context, ILogger<TagCharacterHandler> logger, ITaggingService taggingService)
    {
        _context = context;
        _logger = logger;
        _taggingService = taggingService;
    }

    public async Task<ErrorOr<Success>> Handle(TagCharacterCommand request, CancellationToken cancellationToken)
    {
        var character = await _context.Characters.FindAsync([request.CharacterId], cancellationToken);
        if (character == null)
        {
            _logger.LogInformation("Character with ID {ID} not found", request.CharacterId);
            return CharacterErrors.NotFound;
        }
        
        var tag = await _context.Tags.FindAsync([request.TagId], cancellationToken);
        if (tag == null)
        {
            _logger.LogInformation("Tag with ID {ID} not found", request.TagId);
            return TagErrors.TagNotFound;
        }
        
        var taggingResult = await _taggingService.TagEntity(character, tag);
        if (taggingResult.IsError)
        {
            _logger.LogError("Failed to tag character with ID {CharacterID} with tag that has ID {TagID}",
                request.CharacterId, request.TagId);
            return taggingResult.Errors;
        }

        return Result.Success;
    }
}