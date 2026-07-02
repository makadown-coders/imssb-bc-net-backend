using Application.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.Commands.ResetUserPassword;

public sealed class ResetUserPasswordCommandHandler(
    IUserRepository users,
    IUserRefreshTokenRepository refreshTokens,
    IPasswordHasher passwordHasher,
    IClock clock,
    IUnitOfWork unitOfWork) : IRequestHandler<ResetUserPasswordCommand>
{
    public async Task Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(request.TargetUserId, cancellationToken);
        if (user is null)
        {
            throw new ValidationException("El usuario no existe.");
        }

        user.PasswordHash = passwordHasher.Hash(request.NewPassword);
        // Un restablecimiento administrativo debe cerrar las sesiones persistentes del afectado.
        var tokens = await refreshTokens.GetActiveByUserIdAsync(user.Id, cancellationToken);
        foreach (var token in tokens)
        {
            token.Revoke(clock.UtcNow);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
