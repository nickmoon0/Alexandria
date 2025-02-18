using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;

namespace Alexandria.Domain.EntryAggregate;

public class Entry : AggregateRoot, IAuditable, ISoftDeletable
{
    public string? Name { get; private set; }
    public string? Description { get; private set; }

    public Document? Document { get; private set; }
    public List<Comment> Comments { get; private set; } = [];

    private List<Guid> CharacterIds { get; set; } = [];
    
    public Guid CreatedById { get; }
    public DateTime CreatedAtUtc { get; }
    public DateTime? DeletedAtUtc { get; private set; }
    
    private Entry() { }

    private Entry(
        string name,
        string? description,
        Guid createdById,
        DateTime createdAtUtc,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Name = name;
        Description = description;
        
        CreatedById = createdById;
        CreatedAtUtc = createdAtUtc;
    }

    public static ErrorOr<Entry> Create(
        string name,
        Guid createdById,
        IDateTimeProvider dateTimeProvider,
        string? description = null)
    {
        name = name.Trim();
        if (string.IsNullOrEmpty(name))
        {
            return EntryErrors.InvalidName;
        }

        if (createdById == Guid.Empty)
        {
            return EntryErrors.InvalidId;
        }

        if (description is { Length: 0 }) description = null;
        
        return new Entry(name, description, createdById, dateTimeProvider.UtcNow);
    }
    
    public ErrorOr<Updated> AddCharacter(Guid characterId)
    {
        if (characterId == Guid.Empty)
        {
            return DocumentErrors.InvalidCharacterId;
        }

        if (CharacterIds.Contains(characterId))
        {
            return DocumentErrors.CharacterIdAlreadyPresent;
        }
        
        CharacterIds.Add(characterId);
        return Result.Updated;
    }

    public ErrorOr<Updated> RemoveCharacter(Guid characterId)
    {
        if (characterId == Guid.Empty)
        {
            return DocumentErrors.InvalidCharacterId;
        }

        if (!CharacterIds.Contains(characterId))
        {
            return DocumentErrors.CharacterIdNotPresent;
        }
        
        CharacterIds.Remove(characterId);
        return Result.Updated;
    }

    public ErrorOr<Updated> AddComment(Comment comment)
    {
        if (Comments.Contains(comment))
        {
            return Error.Conflict();
        }
        
        Comments.Add(comment);
        return Result.Updated;
    }

    public ErrorOr<Updated> RemoveComment(Comment comment)
    {
        if (!Comments.Contains(comment))
        {
            return Error.NotFound();
        }

        Comments.Remove(comment);
        return Result.Updated;
    }
    
    public ErrorOr<Deleted> Delete(IDateTimeProvider dateTimeProvider)
    {
        if (DeletedAtUtc is not null)
        {
            return EntryErrors.AlreadyDeleted;
        }

        DeletedAtUtc = dateTimeProvider.UtcNow;
        if (Document?.Delete(dateTimeProvider).IsError ?? false)
        {
            return DocumentErrors.AlreadyDeleted;
        }

        if (Comments.Any(comment => comment.Delete(dateTimeProvider).IsError))
        {
            return CommentErrors.AlreadyDeleted;
        }
        
        return Result.Deleted;
    }

    public ErrorOr<Success> RecoverDeleted()
    {
        if (DeletedAtUtc is null)
        {
            return EntryErrors.NotDeleted;
        }

        DeletedAtUtc = null;
        return Result.Success;
    }
}