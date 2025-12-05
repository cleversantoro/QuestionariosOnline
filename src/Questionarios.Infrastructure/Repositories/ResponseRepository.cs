using Microsoft.EntityFrameworkCore;
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

    public Task<Response?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Responses.Include(r => r.Items).FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyList<Response>> GetBySurveyIdAsync(Guid surveyId, CancellationToken ct = default) =>
        await _db.Responses.AsNoTracking().Where(r => r.SurveyId == surveyId).ToListAsync(ct);

    public async Task DeleteAsync(Response response, CancellationToken ct = default)
    {
        _db.Responses.Remove(response);
        await _db.SaveChangesAsync(ct);
    }
}
