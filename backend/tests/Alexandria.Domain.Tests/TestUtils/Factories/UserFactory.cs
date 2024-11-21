using Alexandria.Domain.Common.ValueObjects.Name;
using Alexandria.Domain.UserAggregate;
using ErrorOr;

namespace Alexandria.Domain.Tests.TestUtils.Factories;

public static class UserFactory
{
    public static ErrorOr<User> CreateUser(
        Name? name = null)
    {
        return User.Create(name ?? NameFactory.CreateName().Value);
    }
}