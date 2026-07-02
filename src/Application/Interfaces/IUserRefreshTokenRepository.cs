using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRefreshTokenRepository
{
    Task<UserRefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserRefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(UserRefreshToken refreshToken, CancellationToken cancellationToken = default);
}
