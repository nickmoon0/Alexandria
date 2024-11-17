using Alexandria.Domain.Common;
using Alexandria.Domain.Common.ValueObjects;
using Alexandria.Domain.Common.ValueObjects.Name;
using ErrorOr;

namespace Alexandria.Domain.User;

public class User : AggregateRoot
{
    private Name UsersName { get; } 

    private User() { }

    public static ErrorOr<User> Create()
    {
        throw new NotImplementedException();
    }
}