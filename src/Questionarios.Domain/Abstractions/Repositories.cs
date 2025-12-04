using Questionarios.Domain.Entities;

namespace Questionarios.Domain.Abstractions;

public interface ISurveyRepository
{
    Task<Survey?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Survey survey, CancellationToken ct = default);
    Task UpdateAsync(Survey survey, CancellationToken ct = default);
}

public interface IResponseRepository
{
    Task AddAsync(Response response, CancellationToken ct = default);
}

// Read-model pra gráfico
public record OptionVotes(Guid QuestionId, string QuestionText, Guid OptionId, string OptionText, int Votes);

public interface IResultsRepository
{
    Task<AggregatedResult?> GetBySurveyIdAsync(Guid surveyId, CancellationToken ct = default);
    Task UpsertAsync(AggregatedResult result, CancellationToken ct = default);
    Task<IReadOnlyList<OptionVotes>> GetOptionVotesAsync(Guid surveyId, CancellationToken ct = default);

}
