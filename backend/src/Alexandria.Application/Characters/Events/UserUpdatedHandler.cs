using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.UserAggregate.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Characters.Events;

public class UserUpdatedHandler : INotificationHandler<UserUpdatedEvent>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UserUpdatedHandler> _logger;

    public UserUpdatedHandler(IAppDbContext context, ILogger<UserUpdatedHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(UserUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var user = notification.User;
        var character = await _context.Characters
            .SingleOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);
        if (character == null)
        {
            _logger.LogInformation("No character with a user ID of {ID}", user.Id);
            return;
        }
        
        character.SetName(user.Name.ShallowClone());
        
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Character updated with ID {ID}", character.Id);
    }
}