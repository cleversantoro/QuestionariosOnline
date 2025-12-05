using Microsoft.EntityFrameworkCore;
using Questionarios.Domain.Abstractions;
using Questionarios.Domain.Entities;

namespace Questionarios.Infrastructure.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly QuestionariosDbContext _db;

    public QuestionRepository(QuestionariosDbContext db) => _db = db;

    public Task<Question?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.Id == id, ct);

    public async Task<IReadOnlyList<Question>> GetBySurveyIdAsync(Guid surveyId, CancellationToken ct = default) =>
        await _db.Questions.AsNoTracking().Where(q => q.SurveyId == surveyId).Include(q => q.Options).OrderBy(q => q.Order).ToListAsync(ct);

    public async Task AddAsync(Question question, CancellationToken ct = default)
    {
        await _db.Questions.AddAsync(question, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Question question, CancellationToken ct = default)
    {
        _db.Questions.Update(question);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Question question, CancellationToken ct = default)
    {
        _db.Questions.Remove(question);
        await _db.SaveChangesAsync(ct);
    }

    public Task<Option?> GetOptionByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Options.FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task AddOptionAsync(Option option, CancellationToken ct = default)
    {
        await _db.Options.AddAsync(option, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateOptionAsync(Option option, CancellationToken ct = default)
    {
        _db.Options.Update(option);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteOptionAsync(Option option, CancellationToken ct = default)
    {
        _db.Options.Remove(option);
        await _db.SaveChangesAsync(ct);
    }
}
