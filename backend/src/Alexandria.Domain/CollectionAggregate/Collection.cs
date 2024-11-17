using Alexandria.Domain.Common;
using ErrorOr;

namespace Alexandria.Domain.CollectionAggregate;

public class Collection : TaggableAggregateRoot
{
    private string? _name;
    private readonly List<Guid>? _documentIds = [];
    
    private Collection() { }

    private Collection(string name, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        _name = name;
    }

    public static ErrorOr<Collection> Create(string name)
    {
        if (!CollectionNameValid(name))
        {
            return CollectionErrors.InvalidName;
        }
        return new Collection(name);
    }

    public ErrorOr<Updated> Rename(string name)
    {
        if (!CollectionNameValid(name))
        {
            return CollectionErrors.InvalidName;
        }
        _name = name;
        return Result.Updated;
    }

    public ErrorOr<Updated> AddDocument(Guid documentId)
    {
        if (documentId == Guid.Empty)
        {
            return CollectionErrors.InvalidDocumentId;
        }
        
        _documentIds!.Add(documentId);
        return Result.Updated;
    }

    public ErrorOr<Updated> RemoveDocument(Guid documentId)
    {
        if (!_documentIds!.Contains(documentId))
        {
            return CollectionErrors.DocumentIdNotFound;
        }
        
        _documentIds!.Remove(documentId);
        return Result.Updated;
    }
    
    private static bool CollectionNameValid(string name) => 
        !string.IsNullOrEmpty(name) &&
        !string.IsNullOrWhiteSpace(name) &&
        name.Length <= 100;
}