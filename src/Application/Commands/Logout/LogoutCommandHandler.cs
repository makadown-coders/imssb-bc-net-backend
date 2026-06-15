using Application.Interfaces;
using MediatR;

namespace Application.Commands.Logout;

public sealed class LogoutCommandHandler(
    IUserRefreshTokenRepository refreshTokens,
    IClock clock,
    IUnitOfWork unitOfWork) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await refreshTokens.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (refreshToken is null || refreshToken.IsRevoked)
        {
            return;
        }

        refreshToken.Revoke(clock.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
