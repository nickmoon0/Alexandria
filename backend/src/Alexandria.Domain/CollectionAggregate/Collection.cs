using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Interfaces;
using ErrorOr;

namespace Alexandria.Domain.CollectionAggregate;

public class Collection : TaggableAggregateRoot, ISoftDeletable
{
    private string? Name { get; set; }
    private List<Guid>? DocumentIds { get; init; } = [];
    
    public DateTime? DeletedAtUtc { get; private set; }

    private Collection() { }

    private Collection(string name, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Name = name;
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
        Name = name;
        return Result.Updated;
    }

    public ErrorOr<Updated> AddDocument(Guid documentId)
    {
        if (documentId == Guid.Empty)
        {
            return CollectionErrors.InvalidDocumentId;
        }
        
        DocumentIds!.Add(documentId);
        return Result.Updated;
    }

    public ErrorOr<Updated> RemoveDocument(Guid documentId)
    {
        if (!DocumentIds!.Contains(documentId))
        {
            return CollectionErrors.DocumentIdNotFound;
        }
        
        DocumentIds!.Remove(documentId);
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
    
    private static bool CollectionNameValid(string name) => 
        !string.IsNullOrEmpty(name) &&
        !string.IsNullOrWhiteSpace(name) &&
        name.Length <= 100;
}