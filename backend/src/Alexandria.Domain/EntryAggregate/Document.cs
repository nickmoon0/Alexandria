using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;

namespace Alexandria.Domain.EntryAggregate;

public class Document : Entity, IAuditable, ISoftDeletable
{
    public string? Name { get; private set; }
    public string? Description { get; private set;}
    
    public string? ImagePath { get; private set; }
    public byte[]? Data { get; private set; } // Data ignored by EF Core
    
    public Guid CreatedById { get; }
    public DateTime CreatedAtUtc { get; }
    
    public DateTime? DeletedAtUtc { get; private set; }

    private Document() { }

    private Document(
        string name,
        byte[] data,
        string imagePath,
        Guid createdById,
        DateTime utcNow,
        string? description = null,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        Name = name;
        Data = data;
        ImagePath = imagePath;
        CreatedById = createdById;
        CreatedAtUtc = utcNow;
        Description = description;
        
        DeletedAtUtc = null;
    }

    public static ErrorOr<Document> Create(
        string documentName,
        byte[] data,
        string imagePath,
        Guid createdById,
        IDateTimeProvider dateTimeProvider,
        string? description = null)
    {
        var errorList = new List<Error>();
        
        // Validate document name
        if (!DocumentNameValid(documentName))
        {
            errorList.Add(DocumentErrors.InvalidDocumentName);
        }
        
        // Validate array isn't empty
        if (data.Length <= 0)
        {
            errorList.Add(DocumentErrors.EmptyData);
        }

        if (createdById == Guid.Empty)
        {
            errorList.Add(DocumentErrors.InvalidUserId);
        }

        if (errorList.Count != 0)
        {
            return errorList;
        }
        
        return new Document(documentName, data, imagePath, createdById, dateTimeProvider.UtcNow, description);
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

    public ErrorOr<Updated> UpdateDescription(string? newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription)) newDescription = null;
        
        Description = newDescription;
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