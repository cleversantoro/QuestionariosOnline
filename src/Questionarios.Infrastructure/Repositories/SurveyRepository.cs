using Microsoft.EntityFrameworkCore;
using Questionarios.Domain.Abstractions;
using Questionarios.Domain.Entities;

namespace Questionarios.Infrastructure.Repositories;

public class SurveyRepository : ISurveyRepository
{
    private readonly QuestionariosDbContext _db;

    public SurveyRepository(QuestionariosDbContext db) => _db = db;

    public Task<Survey?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Surveys
           .Include(s => s.Questions)
           .ThenInclude(q => q.Options)
           .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<Survey>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Surveys.AsNoTracking().OrderBy(s => s.StartAt).ToListAsync(ct);

    public async Task AddAsync(Survey survey, CancellationToken ct = default)
    {
        await _db.Surveys.AddAsync(survey, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Survey survey, CancellationToken ct = default)
    {
        _db.Surveys.Update(survey);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Survey survey, CancellationToken ct = default)
    {
        _db.Surveys.Remove(survey);
        await _db.SaveChangesAsync(ct);
    }
}
