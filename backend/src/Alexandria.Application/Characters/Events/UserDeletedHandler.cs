using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.UserAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Characters.Events;

public class UserDeletedHandler : INotificationHandler<UserDeletedEvent>
{
    private readonly ICharacterRepository _characterRepository;
    private readonly ILogger<UserDeletedHandler> _logger;
    
    public UserDeletedHandler(
        ICharacterRepository characterRepository,
        ILogger<UserDeletedHandler> logger)
    {
        _characterRepository = characterRepository;
        _logger = logger;
    }
    
    public async Task Handle(UserDeletedEvent notification, CancellationToken cancellationToken)
    {
        var characterResult = await _characterRepository.FindByUserIdAsync(notification.UserId, cancellationToken);
        if (characterResult.IsError)
        {
            _logger.LogInformation("No character with a user ID of {ID}", notification.UserId);
            return;
        }
        
        var character = characterResult.Value;
        character.SetUserId(null);
        
        var updateResult = await _characterRepository.UpdateAsync(cancellationToken);
        if (updateResult.IsError)
        {
            _logger.LogInformation("Failed to remove User ID on character with ID: {ID}", character.Id);
            return;
        }
        
        _logger.LogInformation("Successfully removed User ID on character with ID: {ID}", character.Id);
    }
}