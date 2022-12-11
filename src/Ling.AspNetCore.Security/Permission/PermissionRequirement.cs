using Microsoft.AspNetCore.Authorization;

namespace Ling.AspNetCore.Security.Permission;

/// <summary>
/// Represents a permission authorization requirement.
/// </summary>
internal class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// The permission value.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Initialize a <see cref="PermissionRequirement"/>.
    /// </summary>
    /// <param name="permission">The permission value.</param>
    public PermissionRequirement(string permission) => Value = permission;
}
