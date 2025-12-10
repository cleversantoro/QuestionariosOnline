using Microsoft.EntityFrameworkCore;
using Questionarios.Domain.Abstractions;
using Questionarios.Domain.Entities;

namespace Questionarios.Infrastructure.Repositories;

public class ResultsRepository : IResultsRepository
{
    private readonly QuestionariosDbContext _db;

    public ResultsRepository(QuestionariosDbContext db) => _db = db;

    public async Task<AggregatedResult?> GetBySurveyIdAsync(Guid surveyId, CancellationToken ct = default) =>
        await _db.AggregatedResults
            .AsNoTracking()
            .Where(ar => ar.SurveyId == surveyId)
            .OrderByDescending(ar => ar.Votes)
            .FirstOrDefaultAsync(ct);

    public async Task UpsertAsync(AggregatedResult result, CancellationToken ct = default)
    {
        var existing = await _db.AggregatedResults
            .FirstOrDefaultAsync(ar =>
                ar.SurveyId == result.SurveyId &&
                ar.QuestionId == result.QuestionId &&
                ar.OptionId == result.OptionId, ct);

        if (existing is null)
        {
            result.IncrementVote();
            await _db.AggregatedResults.AddAsync(result, ct);
        }
        else
        {
            existing.IncrementVote();
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<OptionVotes>> GetOptionVotesAsync(Guid surveyId, CancellationToken ct = default)
    {
        var grouped =
            await (from ar in _db.AggregatedResults
                   join q in _db.Questions on ar.QuestionId equals q.Id
                   join o in _db.Options on ar.OptionId equals o.Id
                   where ar.SurveyId == surveyId
                   select new OptionVotes(
                       ar.QuestionId,
                       q.Text,
                       ar.OptionId,
                       o.Text,
                       ar.Votes
                   )).ToListAsync(ct);

        return grouped;
    }
}
