using LingDev.AspNetCore.Security.Abstractions;
using LingDev.AspNetCore.Security.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace LingDev.AspNetCore.Security.Route;

/// <summary>
/// Represents a route authorization handler.
/// </summary>
internal class RouteAuthorizationHandler : AuthorizationHandler<RouteRequirement>, IAuthorizationRequirement
{
    private readonly IRouteValidator _routeValidator;

    /// <summary>
    /// Initialize a <see cref="RouteAuthorizationHandler"/>.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
    public RouteAuthorizationHandler(IServiceProvider serviceProvider)
    {
        _routeValidator = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IRouteValidator>();
    }

    /// <inheritdoc/>
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RouteRequirement requirement)
    {
        var result = await _routeValidator.CanAccessAsync(context.User, requirement.Value);
        if (result)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail(new AuthorizationFailureReason(this, "Permission denied."));
        }
    }
}