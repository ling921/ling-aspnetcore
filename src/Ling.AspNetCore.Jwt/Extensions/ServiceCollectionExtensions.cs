using Ling.AspNetCore.Security.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Ling.AspNetCore.Jwt.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register the jwt token authentication service.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="setupAction">A delegate that allows configuring <see cref="JwtOptions"/>.</param>
    public static AuthenticationBuilder AddJwtAuthentication(this IServiceCollection services, Action<JwtOptions> setupAction)
    {
        ArgumentNullException.ThrowIfNull(setupAction);

        services.Configure(setupAction);
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, JwtBearerOptionsConfigurator>());

        services.AddScoped<JwtAuthorizationEvents>();
        services.AddScoped<IAuthorizeService, JwtAuthorizeService>();

        var builder = services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        });
        builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, _ => { });

        return builder;
    }
}
