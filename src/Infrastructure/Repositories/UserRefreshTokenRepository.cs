using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class UserRefreshTokenRepository(AppDbContext dbContext) : IUserRefreshTokenRepository
{
    public Task<UserRefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return dbContext.UserRefreshTokens
            .Include(refreshToken => refreshToken.User)
            .ThenInclude(user => user!.UserRoles)
            .ThenInclude(userRole => userRole.Role)
            .FirstOrDefaultAsync(refreshToken => refreshToken.Token == token, cancellationToken);
    }

    public Task AddAsync(UserRefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        return dbContext.UserRefreshTokens.AddAsync(refreshToken, cancellationToken).AsTask();
    }

    public async Task<IReadOnlyList<UserRefreshToken>> GetActiveByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.UserRefreshTokens
            .Where(token => token.UserId == userId && !token.IsRevoked)
            .ToListAsync(cancellationToken);
    }
}
