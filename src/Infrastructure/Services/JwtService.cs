using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public sealed class JwtService(IOptions<JwtSettings> jwtOptions, IClock clock) : ITokenService
{
    private readonly JwtSettings _settings = jwtOptions.Value;

    public TokenPair GenerateTokens(User user)
    {
        var now = clock.UtcNow;
        var accessTokenExpiresUtc = now.AddHours(_settings.AccessTokenHours);
        var refreshTokenExpiresUtc = now.AddDays(_settings.RefreshTokenDays);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: accessTokenExpiresUtc,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new TokenPair(
            new JwtSecurityTokenHandler().WriteToken(token),
            CreateRefreshToken(),
            accessTokenExpiresUtc,
            refreshTokenExpiresUtc);
    }

    private static string CreateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
    }
}
