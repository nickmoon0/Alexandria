using Alexandria.Domain.Common.ValueObjects.Name;
using ErrorOr;

namespace Alexandria.Domain.Common.Entities;

public class User : Entity
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