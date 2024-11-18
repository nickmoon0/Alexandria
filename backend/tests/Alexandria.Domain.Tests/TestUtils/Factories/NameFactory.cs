using Alexandria.Domain.Common.ValueObjects.Name;
using Alexandria.Domain.Tests.TestConstants;
using ErrorOr;

namespace Alexandria.Domain.Tests.TestUtils.Factories;

public static class NameFactory
{
    public static ErrorOr<Name> CreateName(
        string? firstName = null,
        string? lastName = null,
        string? middleName = null)
    {
        return Name.Create(
            firstName ?? Constants.Name.FirstName,
            lastName ?? Constants.Name.LastName,
            middleName ?? Constants.Name.MiddleName);
    }
}