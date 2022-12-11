using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Ling.AspNetCore.Security.Permission;

/// <summary>
/// Represents a permission authorization policy provider.
/// </summary>
internal class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _options;

    /// <summary>
    /// Initialize a <see cref="PermissionAuthorizationPolicyProvider"/>.
    /// </summary>
    /// <param name="options">The authorization options.</param>
    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options) => _options = options.Value;

    /// <inheritdoc/>
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Check static policies first
        var policy = await base.GetPolicyAsync(policyName);

        if (policy == null)
        {
            policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

            // Add policy to the AuthorizationOptions, so we don't have to re-create it each time
            _options.AddPolicy(policyName, policy);
        }

        return policy;
    }
}
