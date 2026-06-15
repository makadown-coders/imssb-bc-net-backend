using Application.DTOs;
using MediatR;

namespace Application.Queries.RefreshToken;

public sealed record RefreshTokenQuery(string RefreshToken) : IRequest<AuthResponse>;
