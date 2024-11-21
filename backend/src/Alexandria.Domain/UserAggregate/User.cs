using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.Common.ValueObjects.Name;
using ErrorOr;

namespace Alexandria.Domain.UserAggregate;

public class User : AggregateRoot, ISoftDeletable
{
    private Name? Name { get; set; }
    public DateTime? DeletedAtUtc { get; private set; }

    private User() { }

    private User(Name name, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Name = name;
    }
    public static ErrorOr<User> Create(Name name)
    {
        return new User(name);
    }

    public ErrorOr<Deleted> Delete(IDateTimeProvider dateTimeProvider)
    {
        if (DeletedAtUtc.HasValue)
        {
            return UserErrors.AlreadyDeleted;
        }

        DeletedAtUtc = dateTimeProvider.UtcNow;
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