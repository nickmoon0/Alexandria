using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.EntryAggregate;
using Alexandria.Domain.Tests.TestConstants;
using ErrorOr;

namespace Alexandria.Domain.Tests.TestUtils.Factories;

public static class EntryFactory
{
    public static ErrorOr<Entry> CreateEntry(
        string? name = null,
        Guid? createdById = null,
        IDateTimeProvider? dateTimeProvider = null,
        string? description = null,
        Document? document = null)
    {
        return Entry.Create(
            name ?? Constants.Entry.Name,
            createdById ?? Constants.Entry.CreatedById,
            dateTimeProvider ?? Constants.Entry.DateTimeProvider,
            description ?? Constants.Entry.Description);
    }
}