using Alexandria.Application.Common.Roles;

namespace Alexandria.CoreApi.Common;

public abstract class EndpointBase
{
    protected static Guid? UserId { get; private set; }
    protected static IReadOnlyList<Role> Roles { get; private set; } = [];
    public static void InitializeUserId(Guid? userId)
    {
        UserId = userId;
    }
    public static void InitializeRoles(List<Role> roles)
    {
        Roles = roles;
    }
}