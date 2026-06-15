using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Configuration;

public sealed class JwtSettings
{
    [Required]
    [MinLength(32)]
    public string SecretKey { get; init; } = string.Empty;

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Range(1, 24)]
    public int AccessTokenHours { get; init; } = 12;

    [Range(1, 30)]
    public int RefreshTokenDays { get; init; } = 7;
}
