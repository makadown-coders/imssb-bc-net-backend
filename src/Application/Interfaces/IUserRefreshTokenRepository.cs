using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRefreshTokenRepository
{
    Task<UserRefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task AddAsync(UserRefreshToken refreshToken, CancellationToken cancellationToken = default);
}
