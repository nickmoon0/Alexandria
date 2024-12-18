using Alexandria.Domain.CharacterAggregate;
using ErrorOr;

namespace Alexandria.Application.Common.Interfaces;

public interface ICharacterRepository
{
    public Task<ErrorOr<Success>> AddAsync(Character character, CancellationToken cancellationToken);
    public Task<ErrorOr<Character>> FindByIdAsync(Guid characterId, CancellationToken cancellationToken);
    public Task<ErrorOr<Character>> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    public Task<ErrorOr<Success>> UpdateAsync(CancellationToken cancellationToken);
}