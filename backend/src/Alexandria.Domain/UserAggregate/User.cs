using Alexandria.Domain.Common;
using Alexandria.Domain.Common.ValueObjects.Name;
using ErrorOr;

namespace Alexandria.Domain.UserAggregate;

public class User : AggregateRoot
{
    public Name? Name { get; } 

    private User() { }

    private User(Guid id, Name name) : base(id)
    {
        Name = name;
    }
    public static ErrorOr<User> Create(Name name)
    {
        return new User(Guid.NewGuid(), name);
    }
}