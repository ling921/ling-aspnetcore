using LingDev.AspNetCore.Security.Abstractions;
using LingDev.AspNetCore.Security.Permission;
using LingDev.AspNetCore.Security.Route;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace LingDev.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add permission-based authorization.
    /// </summary>
    /// <typeparam name="TPermissionValidator">The type of permission-based authentication validator.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns>A <see cref="AuthenticationBuilder"/> that can be used to further configure authentication.</returns>
    public static AuthenticationBuilder AddPermissionBasedAuthentication<TPermissionValidator>(this IServiceCollection services)
        where TPermissionValidator : class, IPermissionValidator
    {
        services.AddScoped<IPermissionValidator, TPermissionValidator>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services.AddAuthentication();
    }

    /// <summary>
    /// Add permission-based authorization.
    /// </summary>
    /// <typeparam name="TPermissionValidator">The type of permission-based authentication validator.</typeparam>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <returns>A <see cref="AuthenticationBuilder"/> that can be used to further configure authentication.</returns>
    public static AuthenticationBuilder AddPermissionBasedAuthentication<TPermissionValidator>(this AuthenticationBuilder builder)
        where TPermissionValidator : class, IPermissionValidator
    {
        builder.Services.AddScoped<IPermissionValidator, TPermissionValidator>();
        builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return builder;
    }

    /// <summary>
    /// Add route-based authorization.
    /// </summary>
    /// <typeparam name="TRouteValidator">The type of route-based authentication validator.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns>A <see cref="AuthenticationBuilder"/> that can be used to further configure authentication.</returns>
    public static AuthenticationBuilder AddRouterBasedAuthentication<TRouteValidator>(this IServiceCollection services)
        where TRouteValidator : class, IRouteValidator
    {
        services.AddHttpContextAccessor();

        services.AddScoped<IRouteValidator, TRouteValidator>();
        services.AddSingleton<IAuthorizationPolicyProvider, RouteAuthorizationPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, RouteAuthorizationHandler>();

        return services.AddAuthentication();
    }

    /// <summary>
    /// Add route-based authorization.
    /// </summary>
    /// <typeparam name="TRouteValidator">The type of route-based authentication validator.</typeparam>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <returns>A <see cref="AuthenticationBuilder"/> that can be used to further configure authentication.</returns>
    public static AuthenticationBuilder AddRouterBasedAuthentication<TRouteValidator>(this AuthenticationBuilder builder)
        where TRouteValidator : class, IRouteValidator
    {
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<IRouteValidator, TRouteValidator>();
        builder.Services.AddSingleton<IAuthorizationPolicyProvider, RouteAuthorizationPolicyProvider>();
        builder.Services.AddSingleton<IAuthorizationHandler, RouteAuthorizationHandler>();

        return builder;
    }
}
