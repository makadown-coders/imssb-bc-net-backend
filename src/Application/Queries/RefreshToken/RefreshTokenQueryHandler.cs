using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Queries.RefreshToken;

public sealed class RefreshTokenQueryHandler(
    IUserRefreshTokenRepository refreshTokens,
    ITokenService tokenService,
    IClock clock,
    IUnitOfWork unitOfWork) : IRequestHandler<RefreshTokenQuery, AuthResponse>
{
    public async Task<AuthResponse> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var currentToken = await refreshTokens.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (currentToken?.User is null || !currentToken.User.IsActive || !currentToken.IsActive(clock.UtcNow))
        {
            throw new ValidationException("Invalid refresh token.");
        }

        currentToken.Revoke(clock.UtcNow);
        var tokenPair = tokenService.GenerateTokens(currentToken.User);

        await refreshTokens.AddAsync(new UserRefreshToken
        {
            UserId = currentToken.UserId,
            Token = tokenPair.RefreshToken,
            ExpiresUtc = tokenPair.RefreshTokenExpiresUtc,
            CreatedDate = clock.UtcNow
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            tokenPair.AccessToken,
            tokenPair.RefreshToken,
            tokenPair.AccessTokenExpiresUtc,
            tokenPair.RefreshTokenExpiresUtc);
    }
}
