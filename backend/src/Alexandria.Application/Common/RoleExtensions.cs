using Alexandria.Application.Common.Roles;

namespace Alexandria.Application.Common;

public static class RoleExtensions
{
    // This extension method works on any IReadOnlyList of Role (or its subclasses)
    public static bool ContainsRole<T>(this IReadOnlyList<T> roles, Role role)
        where T : Role
    {
        ArgumentNullException.ThrowIfNull(role);
        
        // Compare the concrete types.
        return roles.Any(r => r.GetType() == role.GetType());
    }
}