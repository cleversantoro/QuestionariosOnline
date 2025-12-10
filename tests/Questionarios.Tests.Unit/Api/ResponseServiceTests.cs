using Questionarios.Application.Abstractions;
using Questionarios.Application.DTOs;
using Questionarios.Application.Services;
using Questionarios.Domain.Abstractions;
using Questionarios.Domain.Entities;

namespace Questionarios.Tests.Unit.Api;

public class ResponseServiceTests
{
    [Fact]
    public async Task SubmitAsync_SavesResponseWithTimestamp_WhenSurveyIsActive()
    {
        var surveyId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var optionId = Guid.NewGuid();

        var survey = new Survey("Pesquisa ativa", DateTime.UtcNow.AddMinutes(-10), DateTime.UtcNow.AddMinutes(10));
        // Forcamos o Id para casar com o solicitado
        typeof(Survey).GetProperty(nameof(Survey.Id))!.SetValue(survey, surveyId);

        var question = new Question(surveyId, "Qual sua opÇõÇœo?");
        typeof(Question).GetProperty(nameof(Question.Id))!.SetValue(question, questionId);
        var option = new Option(question.Id, "OpÇõÇœo A");
        typeof(Option).GetProperty(nameof(Option.Id))!.SetValue(option, optionId);

        survey.Questions.Add(question);
        question.Options.Add(option);

        var repo = new FakeSurveyRepository(survey);
        var responseRepo = new FakeResponseRepository();
        var resultsRepo = new FakeResultsRepository();
        var clock = new FixedClock(DateTime.UtcNow);

        var service = new ResponseService(repo, responseRepo, resultsRepo, clock);

        var dto = new ResponseCreateDto(
            surveyId,
            new List<ResponseItemDto> { new(questionId, optionId) }
        );

        var result = await service.SubmitAsync(dto, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(surveyId, result.SurveyId);
        Assert.Equal(clock.UtcNow, result.AnsweredAt);

        var saved = responseRepo.Stored.Single();
        Assert.Equal(result.Id, saved.Id);
        Assert.Equal(clock.UtcNow, saved.AnsweredAt);
        var savedItem = Assert.Single(saved.Items);
        Assert.Equal(questionId, savedItem.QuestionId);
        Assert.Equal(optionId, savedItem.OptionId);

        var aggregated = resultsRepo.Stored.Single();
        Assert.Equal(surveyId, aggregated.SurveyId);
        Assert.Equal(questionId, aggregated.QuestionId);
        Assert.Equal(optionId, aggregated.OptionId);
        Assert.Equal(1, aggregated.Votes);
    }

    private sealed class FakeSurveyRepository : ISurveyRepository
    {
        private readonly Dictionary<Guid, Survey> _store;

        public FakeSurveyRepository(params Survey[] surveys)
        {
            _store = surveys.ToDictionary(s => s.Id, s => s);
        }

        public Task<Survey?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            Task.FromResult(_store.TryGetValue(id, out var survey) ? survey : null);

        public Task<IReadOnlyList<Survey>> GetAllAsync(CancellationToken ct = default) =>
            Task.FromResult((IReadOnlyList<Survey>)_store.Values.ToList());

        public Task AddAsync(Survey survey, CancellationToken ct = default)
        {
            _store[survey.Id] = survey;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Survey survey, CancellationToken ct = default)
        {
            _store[survey.Id] = survey;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Survey survey, CancellationToken ct = default)
        {
            _store.Remove(survey.Id);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeResponseRepository : IResponseRepository
    {
        public List<Response> Stored { get; } = new();

        public Task AddAsync(Response response, CancellationToken ct = default)
        {
            Stored.Add(response);
            return Task.CompletedTask;
        }

        public Task<Response?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            Task.FromResult<Response?>(Stored.FirstOrDefault(r => r.Id == id));

        public Task<IReadOnlyList<Response>> GetBySurveyIdAsync(Guid surveyId, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<Response>>(Stored.Where(r => r.SurveyId == surveyId).ToList());

        public Task DeleteAsync(Response response, CancellationToken ct = default)
        {
            Stored.Remove(response);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeResultsRepository : IResultsRepository
    {
        public List<AggregatedResult> Stored { get; } = new();

        public Task<AggregatedResult?> GetBySurveyIdAsync(Guid surveyId, CancellationToken ct = default) =>
            Task.FromResult<AggregatedResult?>(Stored.FirstOrDefault(r => r.SurveyId == surveyId));

        public Task UpsertAsync(AggregatedResult result, CancellationToken ct = default)
        {
            var existing = Stored.FirstOrDefault(r =>
                r.SurveyId == result.SurveyId &&
                r.QuestionId == result.QuestionId &&
                r.OptionId == result.OptionId);

            if (existing is null)
            {
                result.IncrementVote();
                Stored.Add(result);
            }
            else
            {
                existing.IncrementVote();
            }

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<OptionVotes>> GetOptionVotesAsync(Guid surveyId, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<OptionVotes>>(Array.Empty<OptionVotes>());
    }

    private sealed class FixedClock : IDateTimeProvider
    {
        public FixedClock(DateTime now) => UtcNow = now;
        public DateTime UtcNow { get; }
    }
}
