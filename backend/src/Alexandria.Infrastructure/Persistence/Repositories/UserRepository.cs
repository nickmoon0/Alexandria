using ErrorOr;
using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.UserAggregate;

namespace Alexandria.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<ErrorOr<Success>> AddAsync(User user, CancellationToken cancellationToken)
    {
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    public async Task<ErrorOr<User>> FindByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync([userId], cancellationToken);
        if (user == null)
        {
            return Error.NotFound();
        }

        return user;
    }
}