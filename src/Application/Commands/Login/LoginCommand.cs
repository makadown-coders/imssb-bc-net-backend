using Application.DTOs;
using MediatR;

namespace Application.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
