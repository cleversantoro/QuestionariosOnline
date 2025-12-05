using Microsoft.EntityFrameworkCore;
using Questionarios.Domain.Abstractions;
using Questionarios.Domain.Entities;

namespace Questionarios.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly QuestionariosDbContext _db;

    public UserRepository(QuestionariosDbContext db) => _db = db;

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _db.Users.AddAsync(user, ct);
        await _db.SaveChangesAsync(ct);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Users.AsNoTracking().OrderBy(u => u.Name).ToListAsync(ct);

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Remove(user);
        await _db.SaveChangesAsync(ct);
    }
}
