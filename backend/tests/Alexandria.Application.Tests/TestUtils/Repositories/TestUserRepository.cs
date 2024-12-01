using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.UserAggregate;
using ErrorOr;

namespace Alexandria.Application.Tests.TestUtils.Repositories;

public class TestUserRepository : IUserRepository
{
    public readonly Dictionary<Guid, User> Users;

    public TestUserRepository(List<User> users)
    {
        Users = users.ToDictionary(u => u.Id);
    }
    
    public Task<ErrorOr<Success>> AddAsync(User user, CancellationToken cancellationToken)
    {
        Users.Add(user.Id, user);
        return Task.FromResult<ErrorOr<Success>>(Result.Success);
    }

    public Task<ErrorOr<User>> FindByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        Users.TryGetValue(userId, out var userEntity);

        return userEntity == null ? 
            Task.FromResult<ErrorOr<User>>(Error.NotFound()) : 
            Task.FromResult<ErrorOr<User>>(userEntity);
    }

    public Task<ErrorOr<Success>> UpdateAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}