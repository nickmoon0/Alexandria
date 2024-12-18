using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.Common.ValueObjects.Name;
using Alexandria.Domain.UserAggregate.Events;
using ErrorOr;

namespace Alexandria.Domain.UserAggregate;

public class User : AggregateRoot, ISoftDeletable
{
    public Name Name { get; private set; } = null!;
    public DateTime? DeletedAtUtc { get; private set; }

    private User() { }

    private User(Name name, Guid id) : base(id)
    {
        Name = name;
        DomainEvents.Add(new UserCreatedEvent(this));
    }
    public static ErrorOr<User> Create(Guid id, Name name)
    {
        return new User(name, id);
    }

    public ErrorOr<Updated> UpdateName(Name name)
    {
        Name = name;
        DomainEvents.Add(new UserUpdatedEvent(this));
        return Result.Updated;
    }
    
    public ErrorOr<Deleted> Delete(IDateTimeProvider dateTimeProvider)
    {
        if (DeletedAtUtc.HasValue)
        {
            return UserErrors.AlreadyDeleted;
        }

        DeletedAtUtc = dateTimeProvider.UtcNow;
        DomainEvents.Add(new UserDeletedEvent(Id));
        
        return Result.Deleted;
    }

    public ErrorOr<Success> RecoverDeleted()
    {
        if (!DeletedAtUtc.HasValue)
        {
            return UserErrors.NotDeleted;
        }

        DeletedAtUtc = null;
        return Result.Success;
    }
}