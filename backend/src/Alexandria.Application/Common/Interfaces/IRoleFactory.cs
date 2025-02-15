using Alexandria.Application.Common.Roles;

namespace Alexandria.Application.Common.Interfaces;

public interface IRoleFactory
{
    public Role? CreateRoleInstance(string roleName);
}