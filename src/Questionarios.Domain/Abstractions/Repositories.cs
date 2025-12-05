using Questionarios.Domain.Entities;

namespace Questionarios.Domain.Abstractions;

public interface ISurveyRepository
{
    Task<Survey?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Survey>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Survey survey, CancellationToken ct = default);
    Task UpdateAsync(Survey survey, CancellationToken ct = default);
    Task DeleteAsync(Survey survey, CancellationToken ct = default);
}

public interface IResponseRepository
{
    Task AddAsync(Response response, CancellationToken ct = default);
    Task<Response?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Response>> GetBySurveyIdAsync(Guid surveyId, CancellationToken ct = default);
    Task DeleteAsync(Response response, CancellationToken ct = default);
}

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken ct = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
    Task DeleteAsync(User user, CancellationToken ct = default);
}

public interface IQuestionRepository
{
    Task<Question?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Question>> GetBySurveyIdAsync(Guid surveyId, CancellationToken ct = default);
    Task AddAsync(Question question, CancellationToken ct = default);
    Task UpdateAsync(Question question, CancellationToken ct = default);
    Task DeleteAsync(Question question, CancellationToken ct = default);

    Task<Option?> GetOptionByIdAsync(Guid id, CancellationToken ct = default);
    Task AddOptionAsync(Option option, CancellationToken ct = default);
    Task UpdateOptionAsync(Option option, CancellationToken ct = default);
    Task DeleteOptionAsync(Option option, CancellationToken ct = default);
}

// Read-model para grafico
public record OptionVotes(Guid QuestionId, string QuestionText, Guid OptionId, string OptionText, int Votes);

public interface IResultsRepository
{
    Task<AggregatedResult?> GetBySurveyIdAsync(Guid surveyId, CancellationToken ct = default);
    Task UpsertAsync(AggregatedResult result, CancellationToken ct = default);
    Task<IReadOnlyList<OptionVotes>> GetOptionVotesAsync(Guid surveyId, CancellationToken ct = default);

}
