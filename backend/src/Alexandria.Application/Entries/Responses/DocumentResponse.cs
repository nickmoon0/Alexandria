
using Alexandria.Domain.EntryAggregate;

namespace Alexandria.Application.Entries.Responses;

public class DocumentResponse
{
    public Guid? Id { get; init; }
    public Guid? EntryId { get; init; }
    public string? Name { get; init; }
    public string? ImagePath { get; init; }
    public string? FileExtension { get; init; }
    public Guid? CreatedById { get; init; }
    public DateTime? CreatedAtUtc { get; init; }
    public DateTime? DeletedAtUtc { get; init; }

    public static DocumentResponse FromDocument(Document document)
    {
        return new DocumentResponse
        {
            Id = document.Id,
            Name = document.Name,
            ImagePath = document.ImagePath,
            EntryId = document.EntryId,
            FileExtension = document.FileExtension,
            CreatedById = document.CreatedById,
            CreatedAtUtc = document.CreatedAtUtc,
            DeletedAtUtc = document.DeletedAtUtc,
        };
    }
}