using Questionarios.Domain.Abstractions;
using Questionarios.Domain.Entities;

namespace Questionarios.Infrastructure.Repositories;

public class ResponseRepository : IResponseRepository
{
    private readonly QuestionariosDbContext _db;

    public ResponseRepository(QuestionariosDbContext db) => _db = db;

    public async Task AddAsync(Response response, CancellationToken ct = default)
    {
        await _db.Responses.AddAsync(response, ct);
        await _db.SaveChangesAsync(ct);
    }
}
