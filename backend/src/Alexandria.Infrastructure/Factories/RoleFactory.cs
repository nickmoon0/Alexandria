using System.Reflection;
using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Common.Roles;
using Microsoft.Extensions.Logging;

namespace Alexandria.Infrastructure.Factories;

public class RoleFactory : IRoleFactory
{
    private readonly ILogger<RoleFactory> _logger;

    public RoleFactory(ILogger<RoleFactory> logger)
    {
        _logger = logger;
    }

    public Role? CreateRoleInstance(string roleName)
    {
        var roleNamespace = typeof(Role).Namespace;
        if (roleNamespace == null)
        {
            _logger.LogError("Role namespace is null.");
            return null;
        }

        // Fully qualified name
        var fullTypeName = $"{roleNamespace}.{roleName}";

        // Get the correct assembly where the Role subclasses exist
        var assembly = Assembly.GetAssembly(typeof(Role));
        if (assembly == null)
        {
            _logger.LogError("Could not get assembly for Role.");
            return null;
        }

        // Get the type from the correct assembly
        var type = assembly.GetType(fullTypeName);

        // Ensure type is found and inherits from Role
        if (type != null && typeof(Role).IsAssignableFrom(type))
        {
            return Activator.CreateInstance(type) as Role;
        }
        
        _logger.LogError("Invalid role: '{RoleName}'. Must inherit from Role.", roleName);
        return null;
    }
}