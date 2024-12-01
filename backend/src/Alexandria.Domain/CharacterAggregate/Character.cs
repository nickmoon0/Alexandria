using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.Common.ValueObjects.Name;
using ErrorOr;

namespace Alexandria.Domain.CharacterAggregate;

public class Character : TaggableAggregateRoot, IAuditable, ISoftDeletable
{
    public Name Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public Guid? UserId { get; private set; }

    public Guid CreatedById { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    
    public DateTime? DeletedAtUtc { get; private set; }
    
    private Character() { }

    private Character(
        Name name,
        Guid createdById,
        DateTime createdAtUtc,
        string? description,
        Guid? userId,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Name = name;
        Description = description;
        UserId = userId;
        
        CreatedById = createdById;
        CreatedAtUtc = createdAtUtc;
    }

    public static ErrorOr<Character> Create(
        Name name,
        Guid createdById,
        IDateTimeProvider dateTimeProvider,
        string? description = null,
        Guid? userId = null)
    {
        var errors = new List<Error>();
        
        // Check description is in valid state
        description = description?.Trim();
        if (!string.IsNullOrEmpty(description) && description.Length > 2000)
        {
            errors.Add(CharacterErrors.DescriptionTooLong);
        }
        
        // Check if createdById is in valid state
        if (createdById == Guid.Empty)
        {
            errors.Add(CharacterErrors.InvalidUserId);
        }
        
        // Check userId is in valid state
        if (userId == Guid.Empty)
        {
            errors.Add(CharacterErrors.InvalidUserId);
        }
        
        if (errors.Count != 0)
        {
            return errors;
        }
        
        return new Character(name, createdById, dateTimeProvider.UtcNow, description, userId);
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
}