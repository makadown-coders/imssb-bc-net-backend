namespace Application.DTOs;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresUtc,
    DateTime RefreshTokenExpiresUtc);
