using MediatR;

namespace Application.Commands.Logout;

public sealed record LogoutCommand(string RefreshToken) : IRequest;
