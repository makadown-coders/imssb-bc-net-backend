using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Commands.Login;

public sealed class LoginCommandHandler(
    IUserRepository users,
    IUserRefreshTokenRepository refreshTokens,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IClock clock,
    IUnitOfWork unitOfWork) : IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await users.GetByEmailAsync(email, cancellationToken);
        if (user is null || !user.IsActive || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new ValidationException("Invalid email or password.");
        }

        var tokenPair = tokenService.GenerateTokens(user);
        await refreshTokens.AddAsync(new UserRefreshToken
        {
            UserId = user.Id,
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
