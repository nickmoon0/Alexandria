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