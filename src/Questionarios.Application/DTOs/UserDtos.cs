namespace Questionarios.Application.DTOs;

public record UserDto(Guid Id, string Name, string Email);

public record UserCreateDto(string Name, string Email, string Password);

public record UserUpdateDto(string Name, string Email, string? Password);

public record UserLoginDto(string Email, string Password);
