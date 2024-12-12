using Alexandria.Domain.UserAggregate;
using ErrorOr;

namespace Alexandria.Application.Common.Interfaces;

public interface IUserRepository
{
    public Task<ErrorOr<Success>> AddAsync(User user, CancellationToken cancellationToken);
    public Task<ErrorOr<User>> FindByIdAsync(Guid userId, CancellationToken cancellationToken);
    public Task<ErrorOr<IEnumerable<User>>> FindByIdsAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken);
    public Task<ErrorOr<Success>> UpdateAsync(CancellationToken cancellationToken);
    public Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken);
}