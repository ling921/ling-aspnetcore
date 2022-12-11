using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Ling.AspNetCore.Security.Jwt;

/// <summary>
/// Specifies events which the <see cref="JwtBearerHandler"/> invokes to enable developer control over the authentication process.
/// </summary>
public class JwtAuthorizationEvents : JwtBearerEvents
{
    /// <inheritdoc/>
    public override Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        var endpoint = context.HttpContext?.Features?.Get<IEndpointFeature>()?.Endpoint;
        if (endpoint?.Metadata.OfType<AuthorizeAttribute>().Any() == false)
        {
            return Task.CompletedTask;
        }

        var jsonOptions = context.HttpContext!.RequestServices.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;
        var response = new
        {
            Success = false,
            Code = 10001,
            Message = "您的访问令牌已过期，请重新登录！"
        };
        context.Response.Headers["content-type"] = "application/json; charset=utf-8";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions), Encoding.UTF8);
    }

    /// <inheritdoc/>
    public override Task Forbidden(ForbiddenContext context)
    {
        var jsonOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;
        var response = new
        {
            Success = false,
            Code = 10002,
            Message = "您无权访问此接口，可能是权限变更所致，请退出重新登录再尝试~"
        };
        context.Response.Headers["content-type"] = "application/json; charset=utf-8";
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions), Encoding.UTF8);
    }

    /// <inheritdoc/>
    public override Task MessageReceived(MessageReceivedContext context)
    {
        var authorization = context.Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authorization))
        {
            context.NoResult();
            return Task.CompletedTask;
        }

        if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            context.Token = authorization["Bearer ".Length..].Trim();
        }
        if (string.IsNullOrEmpty(context.Token))
        {
            context.NoResult();
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override Task TokenValidated(TokenValidatedContext context)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override Task Challenge(JwtBearerChallengeContext context)
    {
        return Task.CompletedTask;
    }
}
