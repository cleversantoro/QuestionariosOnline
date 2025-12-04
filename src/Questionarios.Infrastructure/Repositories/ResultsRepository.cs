using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Questionarios.Domain.Abstractions;
using Questionarios.Domain.Entities;

namespace Questionarios.Infrastructure.Repositories;

public class ResultsRepository : IResultsRepository
{
    private readonly QuestionariosDbContext _db;

    public ResultsRepository(QuestionariosDbContext db) => _db = db;

    // Esses dois podem ficar “não implementados” se você não estiver usando
    public Task<AggregatedResult?> GetBySurveyIdAsync(Guid surveyId, CancellationToken ct = default)
        => Task.FromResult<AggregatedResult?>(null);

    public Task UpsertAsync(AggregatedResult result, CancellationToken ct = default)
        => Task.CompletedTask;

    // NOVO: cálculo direto sobre Responses/ResponseItems/Options
    public async Task<IReadOnlyList<OptionVotes>> GetOptionVotesAsync(Guid surveyId, CancellationToken ct = default)
    {
        var grouped =
            await (from r in _db.Responses
                   join ri in _db.ResponseItems on r.Id equals ri.ResponseId
                   join q in _db.Questions on ri.QuestionId equals q.Id
                   join o in _db.Options on ri.OptionId equals o.Id
                   where r.SurveyId == surveyId
                   group new { q, o } by new { QuestionId = q.Id, QuestionText = q.Text, OptionId = o.Id, OptionText = o.Text } into g
                   select new OptionVotes(
                       g.Key.QuestionId,   // QuestionId
                       g.Key.QuestionText, // QuestionText
                       g.Key.OptionId,     // OptionId
                       g.Key.OptionText,   // OptionText
                       g.Count()           // Votes
                   )).ToListAsync(ct);

        return grouped;
    }

}
