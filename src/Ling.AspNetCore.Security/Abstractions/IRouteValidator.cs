using System.Security.Claims;

namespace Ling.AspNetCore.Security.Abstractions;

/// <summary>
/// Route-based authentication validator interface.
/// </summary>
public interface IRouteValidator
{
    /// <summary>
    /// Whether the current user can access the route.
    /// </summary>
    /// <param name="user">The <see cref="ClaimsPrincipal"/> for the current user.</param>
    /// <param name="route">The route value.</param>
    /// <returns><see langword="true"/> if the current user can access the route, otherwise <see langword="false"/>.</returns>
    Task<bool> CanAccessAsync(ClaimsPrincipal user, string route);
}
