using Ling.AspNetCore.Security.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Concurrent;

namespace Ling.AspNetCore.Security.Route;

/// <summary>
/// Represents a route authorization policy provider.
/// </summary>
internal class RouteAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    /// <summary>
    /// The default authorization policy.
    /// </summary>
    public static readonly AuthorizationPolicy DefaultPolicy = new AuthorizationPolicyBuilder().RequireAssertion(_ => true).Build();

    private static readonly ConcurrentDictionary<string, AuthorizationPolicy> _cache = new();

    protected readonly IHttpContextAccessor HttpContextAccessor;

    /// <summary>
    /// Initialize a <see cref="RouteAuthorizationPolicyProvider"/>.
    /// </summary>
    /// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/> provides access to the current <see cref="HttpContent"/>.</param>
    public RouteAuthorizationPolicyProvider(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc/>
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return GetCurrentRoutePolicyAsync();
    }

    /// <inheritdoc/>
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return Task.FromResult<AuthorizationPolicy?>(DefaultPolicy);
    }

    /// <inheritdoc/>
    public virtual async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        return await GetRoutePolicyAsync(policyName);
    }

    protected virtual Task<AuthorizationPolicy> GetCurrentRoutePolicyAsync()
    {
        var endpoint = HttpContextAccessor.HttpContext?.Features?.Get<IEndpointFeature>()?.Endpoint;
        return endpoint == null
            ? Task.FromResult(DefaultPolicy)
            : GetRoutePolicyAsync(endpoint as RouteEndpoint);
    }

    protected static Task<AuthorizationPolicy> GetRoutePolicyAsync(string? policyName)
    {
        if (string.IsNullOrEmpty(policyName))
        {
            return Task.FromResult(DefaultPolicy);
        }

        var route = policyName.ToLower();
        if (!_cache.TryGetValue(route, out var policy))
        {
            policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new RouteRequirement(route))
                .Build();
            _cache.TryAdd(route, policy);
        }
        return Task.FromResult(policy);
    }

    protected static Task<AuthorizationPolicy> GetRoutePolicyAsync(RouteEndpoint? routeEndpoint)
    {
        if (routeEndpoint == null)
        {
            return Task.FromResult(DefaultPolicy);
        }

        var route = routeEndpoint.RoutePattern!.RawText!.ToLower();
        if (!_cache.TryGetValue(route, out var policy))
        {
            policy = routeEndpoint.Metadata.OfType<AuthorizeAttribute>().Any()
                ? new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new RouteRequirement(route))
                    .Build()
                : DefaultPolicy;
            _cache.TryAdd(route, policy);
        }
        return Task.FromResult(policy);
    }

}