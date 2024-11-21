using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Interfaces;
using ErrorOr;

namespace Alexandria.Domain.CollectionAggregate;

public class Collection : TaggableAggregateRoot, IAuditable, ISoftDeletable
{
    private string? Name { get; set; }
    private List<Guid>? EntryIds { get; init; } = [];
    
    public Guid CreatedById { get; }
    public DateTime CreatedAtUtc { get; }
    
    public DateTime? DeletedAtUtc { get; private set; }

    private Collection() { }

    private Collection(string name, Guid createdById, DateTime createdAtUtc, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Name = name;
        
        CreatedById = createdById;
        CreatedAtUtc = createdAtUtc;
    }

    public static ErrorOr<Collection> Create(string name, Guid createdById, IDateTimeProvider dateTimeProvider)
    {
        if (!CollectionNameValid(name))
        {
            return CollectionErrors.InvalidName;
        }

        if (createdById == Guid.Empty)
        {
            return CollectionErrors.InvalidUserId;
        }
        
        return new Collection(name, createdById, dateTimeProvider.UtcNow);
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

    public ErrorOr<Updated> AddEntry(Guid entryId)
    {
        if (entryId == Guid.Empty)
        {
            return CollectionErrors.InvalidEntryId;
        }
        
        EntryIds!.Add(entryId);
        return Result.Updated;
    }

    public ErrorOr<Updated> RemoveEntry(Guid entryId)
    {
        if (!EntryIds!.Contains(entryId))
        {
            return CollectionErrors.EntryIdNotFound;
        }
        
        EntryIds!.Remove(entryId);
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