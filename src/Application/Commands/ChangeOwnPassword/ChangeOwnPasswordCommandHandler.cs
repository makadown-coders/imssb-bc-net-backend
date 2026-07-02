using Application.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.Commands.ChangeOwnPassword;

public sealed class ChangeOwnPasswordCommandHandler(
    IUserRepository users,
    IUserRefreshTokenRepository refreshTokens,
    IPasswordHasher passwordHasher,
    IClock clock,
    IUnitOfWork unitOfWork) : IRequestHandler<ChangeOwnPasswordCommand>
{
    public async Task Handle(ChangeOwnPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            throw new ValidationException("User not found or inactive.");
        }

        if (!passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            throw new ValidationException("The current password is incorrect.");
        }

        user.PasswordHash = passwordHasher.Hash(request.NewPassword);
        await RevokeRefreshTokensAsync(user.Id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task RevokeRefreshTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var tokens = await refreshTokens.GetActiveByUserIdAsync(userId, cancellationToken);
        foreach (var token in tokens)
        {
            token.Revoke(clock.UtcNow);
        }
    }
}
