using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Interfaces;
using ErrorOr;

namespace Alexandria.Domain.DocumentAggregate;

public class Document : AggregateRoot
{
    private string? _documentName;
    private byte[]? _data;
    private Guid? _ownerId;
    private DateTime? _createdDateUtc;
    
    private Document() { }

    private Document(
        string documentName,
        byte[] data,
        Guid ownerId,
        DateTime utcNow,
        Guid? id) 
        : base(id ?? Guid.NewGuid())
    {
        _documentName = documentName;
        _data = data;
        _ownerId = ownerId;
        _createdDateUtc = utcNow;
    }

    public static ErrorOr<Document> Create(string documentName, byte[] data, Guid ownerId, IDateTimeProvider dateTimeProvider)
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
            errorList.Add(DocumentErrors.EmptyDocumentData);
        }

        if (ownerId == Guid.Empty)
        {
            errorList.Add(DocumentErrors.InvalidOwnerId);
        }

        if (errorList.Count != 0)
        {
            return errorList;
        }
        
        return new Document(documentName, data, ownerId, dateTimeProvider.UtcNow, Guid.NewGuid());
    }

    public ErrorOr<Updated> Rename(string newName)
    {
        if (!DocumentNameValid(newName))
        {
            return DocumentErrors.InvalidDocumentName;
        }
        
        _documentName = newName;
        return Result.Updated;
    }
    
    private static bool DocumentNameValid(string name) =>
        !string.IsNullOrWhiteSpace(name) && 
        !string.IsNullOrEmpty(name) && 
        name.Length <= 100;
}