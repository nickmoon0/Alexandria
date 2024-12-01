using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.UserAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Characters.Events;

public class UserCreatedHandler : INotificationHandler<UserCreatedEvent>
{
    private readonly ICharacterRepository _characterRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<UserCreatedHandler> _logger;
    
    public UserCreatedHandler(
        ICharacterRepository characterRepository,
        IDateTimeProvider dateTimeProvider,
        ILogger<UserCreatedHandler> logger)
    {
        _characterRepository = characterRepository;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }
    
    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        var user = notification.User;
        var characterResult = Character.Create(
            user.Name.ShallowClone(), // Clone or EF Core will track user and character name as same entity
            user.Id,
            _dateTimeProvider,
            null,
            user.Id);

        if (characterResult.IsError)
        {
            _logger.LogError("Failed to create character from user: {UserId}", user.Id);
            return;
        }

        var character = characterResult.Value;
        
        var insertResult = await _characterRepository.AddAsync(character, cancellationToken);
        if (insertResult.IsError)
        {
            _logger.LogError("Failed to insert character from user: {UserId}", user.Id);
            return;
        }
        _logger.LogInformation("Created character from user: {UserId}", user.Id);
    }
}