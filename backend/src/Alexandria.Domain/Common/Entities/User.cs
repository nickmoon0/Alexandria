using Alexandria.Domain.Common.ValueObjects.Name;
using ErrorOr;

namespace Alexandria.Domain.Common.Entities;

public class User : Entity
{
    private Name? Name { get; set; }

    private User() { }

    private User(Name name, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Name = name;
    }
    public static ErrorOr<User> Create(Name name)
    {
        return new User(name);
    }
}