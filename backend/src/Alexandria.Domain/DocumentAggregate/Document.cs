using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Interfaces;
using ErrorOr;

namespace Alexandria.Domain.DocumentAggregate;

public class Document : AggregateRoot
{
    public string? DocumentName { get; }
    public byte[]? Data { get; }
    public Guid? OwnerId { get; }
    public DateTime? CreatedDateUtc { get; }
    
    private Document() { }

    private Document(
        string documentName,
        byte[] data,
        Guid ownerId,
        DateTime utcNow,
        Guid? id) 
        : base(id ?? Guid.NewGuid())
    {
        DocumentName = documentName;
        Data = data;
        OwnerId = ownerId;
        CreatedDateUtc = utcNow;
    }

    public static ErrorOr<Document> Create(string documentName, byte[] data, Guid ownerId, IDateTimeProvider dateTimeProvider)
    {
        var errorList = new List<Error>();
        
        // Validate document name
        if (string.IsNullOrWhiteSpace(documentName) || string.IsNullOrEmpty(documentName) || documentName.Length > 100)
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
}