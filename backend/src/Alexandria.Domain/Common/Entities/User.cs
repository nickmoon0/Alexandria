using Alexandria.Domain.Common.ValueObjects.Name;
using ErrorOr;

namespace Alexandria.Domain.Common.Entities;

public class User : Entity
{
    private Name? _name;

    private User() { }

    private User(Name name, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        _name = name;
    }
    public static ErrorOr<User> Create(Name name)
    {
        return new User(name);
    }
}