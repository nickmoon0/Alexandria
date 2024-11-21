using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.EntryAggregate;
using Alexandria.Domain.Tests.TestConstants;
using ErrorOr;

namespace Alexandria.Domain.Tests.TestUtils.Factories;

public static class DocumentFactory
{
    public static ErrorOr<Document> CreateDocument(
        string? name = null,
        byte[]? data = null,
        string? imagePath = null,
        Guid? createdById = null,
        IDateTimeProvider? dateTimeProvider = null,
        string? description = null)
    {
        return Document.Create(
            name ?? Constants.Document.Name,
            data ?? Constants.Document.Data,
            imagePath ?? Constants.Document.ImagePath,
            createdById ?? Constants.Document.CreatedById,
            dateTimeProvider ?? Constants.Document.DateTimeProvider,
            description ?? Constants.Document.Description);
    }
}