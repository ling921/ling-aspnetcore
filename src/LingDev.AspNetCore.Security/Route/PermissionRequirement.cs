using Microsoft.AspNetCore.Authorization;

namespace LingDev.AspNetCore.Security.Permission;

/// <summary>
/// Represents a route authorization requirement.
/// </summary>
internal class RouteRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// The route value.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Initialize a <see cref="RouteRequirement"/>.
    /// </summary>
    /// <param name="route">The route value.</param>
    public RouteRequirement(string route) => Value = route;
}
