using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.CharacterAggregate;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Alexandria.Infrastructure.Persistence.Repositories;

public class CharacterRepository(AppDbContext context) : ICharacterRepository
{
    public async Task<ErrorOr<Success>> AddAsync(Character character, CancellationToken cancellationToken)
    {
        await context.Characters.AddAsync(character, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    public async Task<ErrorOr<Character>> FindByIdAsync(Guid characterId, CancellationToken cancellationToken)
    {
        var character = await context.Characters.FindAsync([characterId], cancellationToken);
        if (character == null)
        {
            return Error.NotFound();
        }

        return character;
    }

    public async Task<ErrorOr<Character>> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var character = await context.Characters
            .SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        if (character == null)
        {
            return Error.NotFound();
        }

        return character;
    }

    public async Task<ErrorOr<Success>> UpdateAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}