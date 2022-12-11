using System.Security.Claims;

namespace Ling.AspNetCore.Jwt;

/// <summary>
/// Interface of authorize service.
/// </summary>
public interface IAuthorizeService
{
    /// <summary>
    /// The method to generate token.
    /// </summary>
    /// <param name="userId">Unique Identifier for user.</param>
    /// <param name="roles">Roles the user has.</param>
    /// <param name="additionalClaims">Additional claims to add into the token.</param>
    /// <returns>The token information.</returns>
    Task<TokenInfo> GenerateTokenAsync(string userId, IEnumerable<string>? roles = null, IEnumerable<Claim>? additionalClaims = null);

    /// <summary>
    /// The method to refresh token.
    /// </summary>
    /// <param name="accessToken">The access token.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <returns>The new token information.</returns>
    Task<TokenInfo> RefreshTokenAsync(string accessToken, string refreshToken);

    /// <summary>
    /// Generate a temporary token.
    /// </summary>
    /// <param name="userId">Unique Identifier for user.</param>
    /// <param name="usage">Usage of the temporary token.</param>
    /// <param name="expires">The temporary token expiration time.</param>
    /// <returns>The temporary token generated.</returns>
    Task<string> GenerateTemporaryTokenAsync(string userId, string usage, TimeSpan? expires = null);

    /// <summary>
    /// Validate the temporary token.
    /// </summary>
    /// <param name="token">The temporary token.</param>
    /// <param name="usage">Usage of the temporary token.</param>
    /// <param name="userId">Unique Identifier for user if the temporary token is valid, otherwise <see cref="string.Empty"/>.</param>
    /// <returns><see langword="true"/> if the temporary token is valid, otherwise <see langword="false"/>.</returns>
    Task<bool> ValidateTemporaryTokenAsync(string token, string usage, out string userId);
}
