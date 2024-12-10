using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.EntryAggregate;
using Alexandria.Domain.Tests.TestConstants;
using ErrorOr;

namespace Alexandria.Domain.Tests.TestUtils.Factories;

public static class DocumentFactory
{
    public static ErrorOr<Document> CreateDocument(
        Guid? entryId = null,
        string? name = null,
        string? fileExtension = null,
        string? imagePath = null,
        Guid? createdById = null,
        IDateTimeProvider? dateTimeProvider = null)
    {
        return Document.Create(
            entryId ?? Constants.Document.EntryId,
            name ?? Constants.Document.Name,
            fileExtension ?? Constants.Document.FileExtension,
            imagePath ?? Constants.Document.ImagePath,
            createdById ?? Constants.Document.CreatedById,
            dateTimeProvider ?? Constants.Document.DateTimeProvider);
    }
}