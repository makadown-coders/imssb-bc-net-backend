using Application.Commands.ChangeOwnPassword;
using Application.Commands.ResetUserPassword;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace Application.Tests;

public sealed class PasswordHandlersTests
{
    [Fact]
    public async Task ChangeOwnPassword_WithValidCurrentPassword_UpdatesHashAndRevokesTokens()
    {
        var utcNow = new DateTime(2026, 7, 2, 12, 0, 0, DateTimeKind.Utc);
        var user = new User { Id = Guid.NewGuid(), PasswordHash = "old-hash", IsActive = true };
        var token = new UserRefreshToken { UserId = user.Id };
        var dependencies = CreateDependencies(user, [token], utcNow);
        dependencies.PasswordHasher.Setup(hasher => hasher.Verify("current-password", "old-hash")).Returns(true);
        dependencies.PasswordHasher.Setup(hasher => hasher.Hash("NewPassword123!")).Returns("new-hash");

        var handler = new ChangeOwnPasswordCommandHandler(
            dependencies.Users.Object, dependencies.RefreshTokens.Object, dependencies.PasswordHasher.Object,
            dependencies.Clock.Object, dependencies.UnitOfWork.Object);

        await handler.Handle(
            new ChangeOwnPasswordCommand(user.Id, "current-password", "NewPassword123!"),
            CancellationToken.None);

        user.PasswordHash.Should().Be("new-hash");
        token.IsRevoked.Should().BeTrue();
        token.RevokedAtUtc.Should().Be(utcNow);
        dependencies.UnitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ChangeOwnPassword_WithIncorrectCurrentPassword_DoesNotPersist()
    {
        var user = new User { Id = Guid.NewGuid(), PasswordHash = "old-hash", IsActive = true };
        var dependencies = CreateDependencies(user, [], DateTime.UtcNow);
        dependencies.PasswordHasher.Setup(hasher => hasher.Verify("wrong-password", "old-hash")).Returns(false);
        var handler = new ChangeOwnPasswordCommandHandler(
            dependencies.Users.Object, dependencies.RefreshTokens.Object, dependencies.PasswordHasher.Object,
            dependencies.Clock.Object, dependencies.UnitOfWork.Object);

        var action = () => handler.Handle(
            new ChangeOwnPasswordCommand(user.Id, "wrong-password", "NewPassword123!"),
            CancellationToken.None);

        await action.Should().ThrowAsync<ValidationException>();
        user.PasswordHash.Should().Be("old-hash");
        dependencies.UnitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ResetUserPassword_UpdatesTargetAndRevokesTokens()
    {
        var user = new User { Id = Guid.NewGuid(), PasswordHash = "old-hash", IsActive = true };
        var token = new UserRefreshToken { UserId = user.Id };
        var dependencies = CreateDependencies(user, [token], DateTime.UtcNow);
        dependencies.PasswordHasher.Setup(hasher => hasher.Hash("AdminReset123!")).Returns("reset-hash");
        var handler = new ResetUserPasswordCommandHandler(
            dependencies.Users.Object, dependencies.RefreshTokens.Object, dependencies.PasswordHasher.Object,
            dependencies.Clock.Object, dependencies.UnitOfWork.Object);

        await handler.Handle(
            new ResetUserPasswordCommand(Guid.NewGuid(), user.Id, "AdminReset123!"),
            CancellationToken.None);

        user.PasswordHash.Should().Be("reset-hash");
        token.IsRevoked.Should().BeTrue();
        dependencies.UnitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void ResetUserPasswordValidator_RejectsResettingOwnPassword()
    {
        var userId = Guid.NewGuid();
        var result = new ResetUserPasswordCommandValidator().Validate(
            new ResetUserPasswordCommand(userId, userId, "AdminReset123!"));

        result.IsValid.Should().BeFalse();
    }

    private static Dependencies CreateDependencies(
        User user,
        IReadOnlyList<UserRefreshToken> tokens,
        DateTime utcNow)
    {
        var users = new Mock<IUserRepository>();
        var refreshTokens = new Mock<IUserRefreshTokenRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();
        var clock = new Mock<IClock>();
        var unitOfWork = new Mock<IUnitOfWork>();
        users.Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        refreshTokens.Setup(repository => repository.GetActiveByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokens);
        clock.SetupGet(current => current.UtcNow).Returns(utcNow);
        return new Dependencies(users, refreshTokens, passwordHasher, clock, unitOfWork);
    }

    private sealed record Dependencies(
        Mock<IUserRepository> Users,
        Mock<IUserRefreshTokenRepository> RefreshTokens,
        Mock<IPasswordHasher> PasswordHasher,
        Mock<IClock> Clock,
        Mock<IUnitOfWork> UnitOfWork);
}
