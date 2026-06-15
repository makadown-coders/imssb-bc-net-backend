namespace Application.DTOs;

public sealed record UserDto(Guid Id, string Email, DateTime CreatedAt);
