using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.Common.Entities.Tag;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Characters.Commands;

public record RemoveTagOnCharacterCommand(Guid CharacterId, Guid TagId) : IRequest<ErrorOr<Success>>;

public class RemoveTagOnCharacterHandler : IRequestHandler<RemoveTagOnCharacterCommand, ErrorOr<Success>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<RemoveTagOnCharacterHandler> _logger;
    private readonly ITaggingService _taggingService;

    public RemoveTagOnCharacterHandler(IAppDbContext context, ILogger<RemoveTagOnCharacterHandler> logger, ITaggingService taggingService)
    {
        _context = context;
        _logger = logger;
        _taggingService = taggingService;
    }

    public async Task<ErrorOr<Success>> Handle(RemoveTagOnCharacterCommand request, CancellationToken cancellationToken)
    {
        var character = await _context.Characters.FindAsync([request.CharacterId], cancellationToken);
        if (character == null)
        {
            _logger.LogInformation("Could not find character with ID {ID}", request.CharacterId);
            return CharacterErrors.NotFound;
        }
        
        var tag = await _context.Tags.FindAsync([request.TagId], cancellationToken);
        if (tag == null)
        {
            _logger.LogInformation("Could not find tag with ID {ID}", request.TagId);
            return TagErrors.TagNotFound;
        }

        var removeTagResult = await _taggingService.RemoveTag(character, tag);
        if (removeTagResult.IsError)
        {
            _logger.LogError("Failed to remove tag with ID {TagID} from character with ID {CharacterID}",
                request.TagId, request.CharacterId);
            return removeTagResult.Errors;
        }

        return Result.Success;
    }
}