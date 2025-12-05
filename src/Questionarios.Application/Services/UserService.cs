using Questionarios.Application.DTOs;
using Questionarios.Domain.Abstractions;
using Questionarios.Domain.Entities;

namespace Questionarios.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> CreateAsync(UserCreateDto dto, CancellationToken ct = default)
    {
        var existing = await _userRepository.GetByEmailAsync(dto.Email, ct);
        if (existing is not null)
            throw new InvalidOperationException("E-mail já cadastrado.");

        var user = new User(dto.Name, dto.Email, dto.Password);
        await _userRepository.AddAsync(user, ct);

        return Map(user);
    }

    public async Task<UserDto?> AuthenticateAsync(UserLoginDto dto, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email, ct);
        if (user is null)
            return null;

        return user.ValidatePassword(dto.Password) ? Map(user) : null;
    }

    public async Task<UserDto> GetAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(id, ct)
                   ?? throw new InvalidOperationException("Usuário não encontrado.");

        return Map(user);
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await _userRepository.GetAllAsync(ct);
        return users.Select(Map).ToList();
    }

    public async Task UpdateAsync(Guid id, UserUpdateDto dto, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(id, ct)
                   ?? throw new InvalidOperationException("Usuário não encontrado.");

        var existing = await _userRepository.GetByEmailAsync(dto.Email, ct);
        if (existing is not null && existing.Id != id)
            throw new InvalidOperationException("E-mail já cadastrado.");

        user.Update(dto.Name, dto.Email);
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            user.SetPassword(dto.Password);
        }

        await _userRepository.UpdateAsync(user, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(id, ct)
                   ?? throw new InvalidOperationException("Usuário não encontrado.");

        await _userRepository.DeleteAsync(user, ct);
    }

    private static UserDto Map(User user) => new(user.Id, user.Name, user.Email);
}
