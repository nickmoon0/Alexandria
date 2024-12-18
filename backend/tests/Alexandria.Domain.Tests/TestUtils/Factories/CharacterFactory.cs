using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.Common.ValueObjects.Name;
using Alexandria.Domain.Tests.TestConstants;
using ErrorOr;

namespace Alexandria.Domain.Tests.TestUtils.Factories;

public static class CharacterFactory
{
    public static ErrorOr<Character> CreateCharacter(
        Name? name = null,
        Guid? createdById = null,
        IDateTimeProvider? dateTimeProvider = null,
        string? description = null,
        Guid? userId = null)
    {
        return Character.Create(
            name ?? NameFactory.CreateName().Value,
            createdById ?? Constants.Character.CreatedById,
            dateTimeProvider ?? Constants.Character.DateTimeProvider,
            description ?? Constants.Character.Description,
            userId);
    }
}