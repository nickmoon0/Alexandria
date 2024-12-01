using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.UserAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Characters.Events;

public class UserUpdatedHandler : INotificationHandler<UserUpdatedEvent>
{
    private readonly ICharacterRepository _characterRepository;
    private readonly ILogger<UserUpdatedHandler> _logger;

    public UserUpdatedHandler(ICharacterRepository characterRepository, ILogger<UserUpdatedHandler> logger)
    {
        _characterRepository = characterRepository;
        _logger = logger;
    }

    public async Task Handle(UserUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var user = notification.User;
        var characterResult = await _characterRepository.FindByUserIdAsync(user.Id, cancellationToken);
        if (characterResult.IsError)
        {
            _logger.LogInformation("No character with a user ID of {ID}", user.Id);
            return;
        }
        
        var character = characterResult.Value;

        character.SetName(user.Name.ShallowClone());
        var updateResult = await _characterRepository.UpdateAsync(cancellationToken);
        if (updateResult.IsError)
        {
            _logger.LogError("Failed to update character with ID of {ID}", character.Id);
            return;
        }
        
        _logger.LogInformation("Character updated with ID {ID}", character.Id);
    }
}