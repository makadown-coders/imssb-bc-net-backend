using Application.Commands.Login;
using Application.Interfaces;
using Application.Queries.RefreshToken;
using Domain.Entities;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace Application.Tests;

public sealed class AuthHandlersTests
{
    [Fact]
    public async Task LoginCommand_Handler_InvalidCredentials_ThrowsValidationException()
    {
        var users = new Mock<IUserRepository>();
        var refreshTokens = new Mock<IUserRefreshTokenRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();
        var tokenService = new Mock<ITokenService>();
        var clock = new Mock<IClock>();
        var unitOfWork = new Mock<IUnitOfWork>();

        users.Setup(repository => repository.GetByEmailAsync("demo@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Email = "demo@example.com", PasswordHash = "hash", IsActive = true });
        passwordHasher.Setup(hasher => hasher.Verify("wrong-password", "hash")).Returns(false);

        var handler = new LoginCommandHandler(
            users.Object,
            refreshTokens.Object,
            passwordHasher.Object,
            tokenService.Object,
            clock.Object,
            unitOfWork.Object);

        var act = () => handler.Handle(new LoginCommand("demo@example.com", "wrong-password"), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RefreshTokenQuery_Handler_ValidToken_ReturnsNewTokens()
    {
        var utcNow = new DateTime(2026, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var user = new User { Id = Guid.NewGuid(), Email = "demo@example.com", IsActive = true };
        var storedToken = new UserRefreshToken
        {
            UserId = user.Id,
            Token = "existing-refresh-token",
            ExpiresUtc = utcNow.AddDays(1),
            User = user
        };

        var refreshTokens = new Mock<IUserRefreshTokenRepository>();
        var tokenService = new Mock<ITokenService>();
        var clock = new Mock<IClock>();
        var unitOfWork = new Mock<IUnitOfWork>();

        refreshTokens.Setup(repository => repository.GetByTokenAsync("existing-refresh-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(storedToken);
        clock.SetupGet(current => current.UtcNow).Returns(utcNow);
        tokenService.Setup(service => service.GenerateTokens(user))
            .Returns(new TokenPair("new-access-token", "new-refresh-token", utcNow.AddHours(12), utcNow.AddDays(7)));

        var handler = new RefreshTokenQueryHandler(
            refreshTokens.Object,
            tokenService.Object,
            clock.Object,
            unitOfWork.Object);

        var result = await handler.Handle(new RefreshTokenQuery("existing-refresh-token"), CancellationToken.None);

        result.AccessToken.Should().Be("new-access-token");
        result.RefreshToken.Should().Be("new-refresh-token");
        storedToken.IsRevoked.Should().BeTrue();
        refreshTokens.Verify(repository => repository.AddAsync(
            It.Is<UserRefreshToken>(token => token.Token == "new-refresh-token" && token.UserId == user.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
