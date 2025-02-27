using Alexandria.Domain.CharacterAggregate;
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

    public List<Character> Characters { get; init; } = [];
    public IReadOnlyList<Guid> CharacterIds => Characters.Select(c => c.Id).ToList();
    
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
    
    /// <summary>
    /// Updates the entry's name.
    /// </summary>
    /// <param name="newName">The new name for the entry.</param>
    /// <returns>An ErrorOr indicating success or a domain error.</returns>
    public ErrorOr<Updated> UpdateName(string newName)
    {
        newName = newName?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(newName))
        {
            return EntryErrors.InvalidName;
        }
        
        Name = newName;
        return Result.Updated;
    }
    
    /// <summary>
    /// Updates the entry's description.
    /// </summary>
    /// <param name="newDescription">The new description for the entry. An empty value is treated as null.</param>
    /// <returns>An ErrorOr indicating success.</returns>
    public ErrorOr<Updated> UpdateDescription(string? newDescription)
    {
        newDescription = newDescription?.Trim();
        if (newDescription is { Length: 0 })
        {
            newDescription = null;
        }
        
        Description = newDescription;
        return Result.Updated;
    }
    
    public ErrorOr<Updated> AddCharacter(Character character)
    {
        if (Characters.Contains(character))
        {
            return DocumentErrors.CharacterIdAlreadyPresent;
        }
        
        Characters.Add(character);
        return Result.Updated;
    }

    public ErrorOr<Updated> RemoveCharacter(Character character)
    {
        if (!Characters.Contains(character))
        {
            return DocumentErrors.CharacterIdNotPresent;
        }
        
        Characters.Remove(character);
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