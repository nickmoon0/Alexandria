using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.UserAggregate.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Users.Events;

public class UserDeletedHandler : INotificationHandler<UserDeletedEvent>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UserDeletedHandler> _logger;
    
    public UserDeletedHandler(
        IAppDbContext context,
        ILogger<UserDeletedHandler> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task Handle(UserDeletedEvent notification, CancellationToken cancellationToken)
    {
        var character = await _context.Characters
            .SingleOrDefaultAsync(x => x.UserId == notification.UserId, cancellationToken);
        if (character == null)
        {
            _logger.LogInformation("No character with a user ID of {ID}", notification.UserId);
            return;
        }
        
        character.SetUserId(null);
        
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Successfully removed User ID on character with ID: {ID}", character.Id);
    }
}