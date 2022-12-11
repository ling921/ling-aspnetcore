using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Ling.AspNetCore.Security.Jwt;

internal class JwtBearerOptionsConfigurator : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly IServiceProvider _serviceProvider;

    public JwtBearerOptionsConfigurator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        var jwtOptions = GetJwtOptions();

        options.RequireHttpsMetadata = options.RequireHttpsMetadata;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256, SecurityAlgorithms.RsaSha256 },
            ValidTypes = new[] { JwtConstants.HeaderType },

            ValidateIssuer = jwtOptions.ValidateIssuer,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = jwtOptions.ValidateAudience,
            ValidAudience = jwtOptions.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
            ValidateIssuerSigningKey = true,

            ValidateLifetime = true,

            RequireSignedTokens = true,
            RequireExpirationTime = true,

            NameClaimType = JwtClaimTypes.Subject,
            RoleClaimType = JwtClaimTypes.Role,

            ClockSkew = TimeSpan.Zero,
        };
        options.MapInboundClaims = false;

        options.EventsType = typeof(JwtAuthorizationEvents);
    }

    private JwtOptions GetJwtOptions()
    {
        using var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<JwtOptions>>().Value;
    }
}
