using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;

namespace Alexandria.Domain.EntryAggregate;

public class Document : Entity, IAuditable, ISoftDeletable
{
    public string? Name { get; private set; }
    public string? FileExtension { get; private set; }
    public string? ImagePath { get; private set; }
    public Guid? EntryId { get; private set; }
    
    public Guid CreatedById { get; }
    public DateTime CreatedAtUtc { get; }
    
    public DateTime? DeletedAtUtc { get; private set; }

    private Document() { }

    private Document(
        Guid entryId,
        string name,
        string fileExtension,
        string imagePath,
        Guid createdById,
        DateTime utcNow,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        Name = name;
        FileExtension = fileExtension;
        ImagePath = imagePath;
        CreatedById = createdById;
        CreatedAtUtc = utcNow;
        
        EntryId = entryId;
        
        DeletedAtUtc = null;
    }

    public static ErrorOr<Document> Create(
        Guid entryId,
        string documentName,
        string fileExtension,
        string imagePath,
        Guid createdById,
        IDateTimeProvider dateTimeProvider)
    {
        var errorList = new List<Error>();

        if (entryId == Guid.Empty)
        {
            errorList.Add(DocumentErrors.InvalidEntryId);
        }
        
        // Validate document name
        if (!DocumentNameValid(documentName))
        {
            errorList.Add(DocumentErrors.InvalidDocumentName);
        }

        if (createdById == Guid.Empty)
        {
            errorList.Add(DocumentErrors.InvalidUserId);
        }

        if (errorList.Count != 0)
        {
            return errorList;
        }
        
        return new Document(entryId, documentName, fileExtension, imagePath, createdById, dateTimeProvider.UtcNow);
    }

    public ErrorOr<Updated> Rename(string newName)
    {
        if (!DocumentNameValid(newName))
        {
            return DocumentErrors.InvalidDocumentName;
        }
        
        Name = newName;
        return Result.Updated;
    }
    
    public ErrorOr<Deleted> Delete(IDateTimeProvider dateTimeProvider)
    {
        if (DeletedAtUtc.HasValue)
        {
            return Error.Failure();
        }
        
        DeletedAtUtc = dateTimeProvider.UtcNow;
        return Result.Deleted;
    }

    public ErrorOr<Success> RecoverDeleted()
    {
        if (!DeletedAtUtc.HasValue)
        {
            return Error.Failure();
        }
        
        DeletedAtUtc = null;
        return Result.Success;
    }
    
    private static bool DocumentNameValid(string name) =>
        !string.IsNullOrWhiteSpace(name) && 
        !string.IsNullOrEmpty(name) && 
        name.Length <= 100;
}