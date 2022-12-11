using System.ComponentModel.DataAnnotations;

namespace Ling.AspNetCore.Security.Jwt;

/// <summary>
/// Options for json web token.
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// aaa
    /// </summary>
    public bool RequireHttpsMetadata { get; set; } = true;

    /// <summary>
    /// The secret.
    /// </summary>
    [Required]
    public string Secret { get; set; } = null!;

    /// <summary>
    /// The issuer.
    /// </summary>
    [Required]
    public string Issuer { get; set; } = null!;

    /// <summary>
    /// The audience.
    /// </summary>
    [Required]
    public string Audience { get; set; } = null!;

    /// <summary>
    /// A flag to indicate whether to validate the issuer.
    /// </summary>
    public bool ValidateIssuer { get; set; } = true;

    /// <summary>
    /// A flag to indicate whether to validate the audience.
    /// </summary>
    public bool ValidateAudience { get; set; } = true;

    /// <summary>
    /// The access token expiration seconds. Default is 3 hours.
    /// </summary>
    public int AccessTokenExpires { get; set; } = 3 * 60 * 60;

    /// <summary>
    /// The refresh token expiration seconds. Default is 15 days.
    /// </summary>
    public int RefreshTokenExpires { get; set; } = 15 * 24 * 60 * 60;
}
