using Alexandria.Domain.Common.ValueObjects.Name;
using Alexandria.Domain.UserAggregate;
using ErrorOr;

namespace Alexandria.Domain.Tests.TestUtils.Factories;

public static class UserFactory
{
    public static ErrorOr<User> CreateUser(
        Name? name = null,
        Guid? id = null)
    {
        return User.Create(
            id ?? Guid.NewGuid(),
            name ?? NameFactory.CreateName().Value);
    }
}