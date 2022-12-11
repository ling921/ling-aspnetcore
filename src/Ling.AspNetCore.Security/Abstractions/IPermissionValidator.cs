using System.Security.Claims;

namespace Ling.AspNetCore.Security.Abstractions;

/// <summary>
/// Permission-based authentication validator interface.
/// </summary>
public interface IPermissionValidator
{
    /// <summary>
    /// Whether the current user has permission for this request.
    /// </summary>
    /// <param name="user">The <see cref="ClaimsPrincipal"/> for the current user.</param>
    /// <param name="permission">The permission name.</param>
    /// <returns><see langword="true"/> if the current user has permission for this request, otherwise <see langword="false"/>.</returns>
    Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission);
}
