using Ling.AspNetCore.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Ling.AspNetCore.Security.Jwt;

/// <summary>
/// The json web token authorize service.
/// </summary>
internal class JwtAuthorizeService : IAuthorizeService
{
    private const string _refreshTokenIdClaimType = "refresh_token_id";

    private readonly ILogger<JwtAuthorizeService> _logger;
    private readonly IDistributedCache _distributedCache;
    private readonly JwtOptions _jwtOptions;
    private readonly JwtBearerOptions _jwtBearerOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtAuthorizeService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="distributedCache">The distributed cache.</param>
    /// <param name="jwtOptions">The options for json web token.</param>
    /// <param name="jwtBearerOptions">The options controls Bearer Authentication handler behavior.</param>
    public JwtAuthorizeService(
        ILogger<JwtAuthorizeService> logger,
        IDistributedCache distributedCache,
        IOptionsSnapshot<JwtOptions> jwtOptions,
        IOptionsSnapshot<JwtBearerOptions> jwtBearerOptions)
    {
        _logger = logger;
        _distributedCache = distributedCache;
        _jwtOptions = jwtOptions.Value;
        _jwtBearerOptions = jwtBearerOptions.Get(JwtBearerDefaults.AuthenticationScheme);
    }

    /// <inheritdoc/>
    public async Task<TokenInfo> GenerateTokenAsync(string userId, IEnumerable<string>? roles = null, IEnumerable<Claim>? additionalClaims = null)
    {
        var refreshTokenId = Guid.NewGuid().ToString("N");
        var rnBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(rnBytes);
        var refreshToken = Convert.ToBase64String(rnBytes);

        var options = new DistributedCacheEntryOptions();
        options.SetAbsoluteExpiration(TimeSpan.FromSeconds(_jwtOptions.RefreshTokenExpires));
        await _distributedCache.SetStringAsync(GetRefreshTokenKey(userId, refreshTokenId), refreshToken, options);

        var claims = new List<Claim>
        {
            new Claim(_jwtBearerOptions.TokenValidationParameters.NameClaimType, userId),
            new Claim(_refreshTokenIdClaimType, refreshTokenId),
        };
        if (roles?.Any() == true)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(_jwtBearerOptions.TokenValidationParameters.RoleClaimType, role));
            }
        }
        if (additionalClaims?.Any() == true)
        {
            claims.AddRange(additionalClaims);
        }

        var expires = DateTime.UtcNow.Add(TimeSpan.FromSeconds(_jwtOptions.AccessTokenExpires));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtBearerOptions.TokenValidationParameters.ValidIssuer,
            Audience = _jwtBearerOptions.TokenValidationParameters.ValidAudience,
            Expires = expires,
            SigningCredentials = credentials,
        };

        var handler = _jwtBearerOptions.SecurityTokenValidators.OfType<JwtSecurityTokenHandler>().FirstOrDefault() ?? new JwtSecurityTokenHandler();
        var securityToken = handler.CreateJwtSecurityToken(tokenDescriptor);
        var accessToken = handler.WriteToken(securityToken);

        _logger.LogDebug("Generated token for user: {userId} \r\naccess_token: {accessToken} \r\nrefresh_token: {refreshToken} \r\nexpires: {expires:yyyy-MM-dd HH:mm:ss}", userId, accessToken, refreshToken, expires);

        return new TokenInfo(accessToken, refreshToken);
    }

    /// <inheritdoc/>
    public async Task<TokenInfo> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var validationParameters = _jwtBearerOptions.TokenValidationParameters.Clone();
        validationParameters.ValidateLifetime = false;

        var handler = _jwtBearerOptions.SecurityTokenValidators.OfType<JwtSecurityTokenHandler>().FirstOrDefault() ?? new JwtSecurityTokenHandler();
        ClaimsPrincipal? principal = null;
        try
        {
            principal = handler.ValidateToken(accessToken, validationParameters, out _);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "An error occurred while validating the token.");
            throw;
        }

        var userId = principal.FindFirstValue(_jwtBearerOptions.TokenValidationParameters.NameClaimType)!;
        var roles = principal.FindAll(_jwtBearerOptions.TokenValidationParameters.RoleClaimType).Select(c => c.Value);
        var claims = principal.Claims
            .Where(c => c.Type != _jwtBearerOptions.TokenValidationParameters.NameClaimType &&
                        c.Type != _jwtBearerOptions.TokenValidationParameters.RoleClaimType &&
                        c.Type != _refreshTokenIdClaimType);
        var refreshTokenId = principal.FindFirstValue(_refreshTokenIdClaimType)!;
        var refreshTokenKey = GetRefreshTokenKey(userId, refreshTokenId);
        var cacheRefreshToken = await _distributedCache.GetStringAsync(refreshTokenKey);

        if (cacheRefreshToken != refreshToken)
        {
            _logger.LogWarning("The refresh token is inconsistent with the cache one.");
            throw new SecurityTokenException("Invalid refresh token.");
        }

        await _distributedCache.RemoveAsync(refreshTokenKey);

        _logger.LogDebug("Refresh token for user: {userId}", userId);

        return await GenerateTokenAsync(userId, roles, claims);
    }

    /// <inheritdoc/>
    public async Task<string> GenerateTemporaryTokenAsync(string userId, string usage, TimeSpan? expires = null)
    {
        if (expires == null || expires <= TimeSpan.Zero)
        {
            expires = TimeSpan.FromMinutes(5);
        }

        var temporaryTokenId = Guid.NewGuid().ToString("N");

        var info = new TemporaryTokenInfo(userId, usage);
        var json = JsonSerializer.Serialize(info);

        var options = new DistributedCacheEntryOptions();
        options.SetAbsoluteExpiration(expires.Value);
        var key = GetTemporaryTokenKey(usage, temporaryTokenId);
        await _distributedCache.SetStringAsync(key, json, options);

        _logger.LogDebug("Generated temporary token for user: {userId}, usage: {usage}, cache_key: {key}", userId, usage, key);

        return temporaryTokenId;
    }

    /// <inheritdoc/>
    public Task<bool> ValidateTemporaryTokenAsync(string token, string usage, out string userId)
    {
        userId = string.Empty;
        var key = GetTemporaryTokenKey(usage, token);
        var json = _distributedCache.GetString(key);
        if (json == null)
        {
            _logger.LogWarning("The temporary token({token}) is not in the cache and may be expired or used.", token);
            throw new SecurityTokenExpiredException();
        }

        var info = JsonSerializer.Deserialize<TemporaryTokenInfo>(json);
        if (info?.Usage != usage)
        {
            _logger.LogWarning("The usage({cache_usage}) of the temporary token is inconsistent with the parameter({param_usage})", info?.Usage, usage);
            throw new SecurityTokenException();
        }

        _distributedCache.Remove(key);
        _logger.LogDebug("Removed the temporary token cache_key: {key}", key);

        userId = info.UserId;
        return Task.FromResult(true);
    }

    private static string GetRefreshTokenKey(string userId, string key) => $"refresh_token_{userId}_{key}";

    private static string GetTemporaryTokenKey(string tokenUsage, string key) => $"temp_token_{tokenUsage}_{key}";

    private class TemporaryTokenInfo
    {
        public TemporaryTokenInfo(string userId, string usage)
        {
            UserId = userId;
            Usage = usage;
        }

        public string UserId { get; set; } = null!;
        public string Usage { get; set; } = null!;
    }
}
