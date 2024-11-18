using Alexandria.Domain.CollectionAggregate;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.Tests.TestConstants;
using ErrorOr;

namespace Alexandria.Domain.Tests.TestUtils.Factories;

public static class CollectionFactory
{
    public static ErrorOr<Collection> CreateCollection(
        string? name = null,
        Guid? createdById = null,
        IDateTimeProvider? dateTimeProvider = null)
    {
        return Collection.Create(
            name ?? Constants.Collection.Name,
            createdById ?? Constants.Collection.CreatedById,
            dateTimeProvider ?? Constants.Collection.DateTimeProvider);
    }
}