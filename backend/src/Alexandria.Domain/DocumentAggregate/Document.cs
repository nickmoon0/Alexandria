using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Interfaces;
using ErrorOr;

namespace Alexandria.Domain.DocumentAggregate;

public class Document : TaggableAggregateRoot, ISoftDeletable
{
    private string? _name;
    private string? _description;
    
    private readonly List<Guid> _characterIds = [];
    
    private string? _imagePath;
    private byte[]? _data;
    
    private Guid? _ownerId;
    private DateTime? _createdDateUtc;

    public DateTime? DeletedAtUtc { get; private set; }

    private Document() { }

    private Document(
        string name,
        byte[] data,
        string imagePath,
        Guid ownerId,
        DateTime utcNow,
        string? description = null,
        Guid? id = null)
        : base(id ?? Guid.NewGuid())
    {
        _name = name;
        _data = data;
        _imagePath = imagePath;
        _ownerId = ownerId;
        _createdDateUtc = utcNow;
        _description = description;
        
        DeletedAtUtc = null;
    }

    public static ErrorOr<Document> Create(
        string documentName,
        byte[] data,
        string imagePath,
        Guid ownerId,
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
        
        return new Document(documentName, data, imagePath, ownerId, dateTimeProvider.UtcNow, description);
    }

    public ErrorOr<Updated> Rename(string newName)
    {
        if (!DocumentNameValid(newName))
        {
            return DocumentErrors.InvalidDocumentName;
        }
        
        _name = newName;
        return Result.Updated;
    }

    public ErrorOr<Updated> UpdateDescription(string? newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription)) newDescription = null;
        
        _description = newDescription;
        return Result.Updated;
    }

    public ErrorOr<Updated> AddCharacter(Guid characterId)
    {
        if (characterId == Guid.Empty)
        {
            return DocumentErrors.InvalidCharacterId;
        }

        if (_characterIds.Contains(characterId))
        {
            return DocumentErrors.CharacterIdAlreadyPresent;
        }
        
        _characterIds.Add(characterId);
        return Result.Updated;
    }

    public ErrorOr<Updated> RemoveCharacter(Guid characterId)
    {
        if (characterId == Guid.Empty)
        {
            return DocumentErrors.InvalidCharacterId;
        }

        if (!_characterIds.Contains(characterId))
        {
            return DocumentErrors.CharacterIdNotPresent;
        }
        
        _characterIds.Remove(characterId);
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