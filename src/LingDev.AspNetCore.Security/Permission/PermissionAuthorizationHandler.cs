using LingDev.AspNetCore.Security.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace LingDev.AspNetCore.Security.Permission;

/// <summary>
/// Represents a permission authorization handler.
/// </summary>
internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>, IAuthorizationRequirement
{
    private readonly IPermissionValidator _permissionValidator;

    /// <summary>
    /// Initialize a <see cref="PermissionAuthorizationHandler"/>.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
    public PermissionAuthorizationHandler(IServiceProvider serviceProvider)
    {
        _permissionValidator = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IPermissionValidator>();
    }

    /// <inheritdoc/>
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var result = await _permissionValidator.HasPermissionAsync(context.User, requirement.Value);
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
