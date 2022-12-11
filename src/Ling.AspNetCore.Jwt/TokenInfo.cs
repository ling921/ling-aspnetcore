namespace Ling.AspNetCore.Jwt;

/// <summary>
/// Represents infomations of jwt token.
/// </summary>
public sealed class TokenInfo
{
    /// <summary>
    /// Initialize a <see cref="TokenInfo"/>.
    /// </summary>
    /// <param name="accessToken">The access token.</param>
    /// <param name="refreshToken">The refresh token.</param>
    public TokenInfo(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    /// <summary>
    /// The access token.
    /// </summary>
    public string AccessToken { get; init; }

    /// <summary>
    /// The refresh token.
    /// </summary>
    public string RefreshToken { get; init; }
}
