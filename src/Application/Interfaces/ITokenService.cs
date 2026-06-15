using Domain.Entities;

namespace Application.Interfaces;

public interface ITokenService
{
    TokenPair GenerateTokens(User user);
}

public sealed record TokenPair(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresUtc,
    DateTime RefreshTokenExpiresUtc);
