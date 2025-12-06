using Questionarios.Application.Abstractions;
using Questionarios.Application.DTOs;
using Questionarios.Application.Services;
using Questionarios.Domain.Abstractions;
using Questionarios.Domain.Entities;

namespace Questionarios.Tests.Unit.Api;

public class ResponseServiceTests
{
    [Fact]
    public async Task SubmitAsync_PublishesMessage_WhenSurveyIsActive()
    {
        var surveyId = Guid.NewGuid();
        var survey = new Survey("Pesquisa ativa", DateTime.UtcNow.AddMinutes(-10), DateTime.UtcNow.AddMinutes(10));
        // Forcamos o Id para casar com o solicitado
        typeof(Survey).GetProperty(nameof(Survey.Id))!.SetValue(survey, surveyId);

        var repo = new FakeSurveyRepository(survey);
        var queue = new FakeQueueClient();
        var clock = new FixedClock(DateTime.UtcNow);

        var service = new ResponseService(repo, queue, clock);

        var dto = new ResponseCreateDto(
            surveyId,
            new List<ResponseItemDto> { new(Guid.NewGuid(), Guid.NewGuid()) }
        );

        await service.SubmitAsync(dto, CancellationToken.None);

        Assert.NotNull(queue.Published);
        Assert.Equal(surveyId, queue.Published!.SurveyId);
        Assert.Single(queue.Published.Items);
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

    private sealed class FakeQueueClient : IQueueClient
    {
        public ResponseMessage? Published { get; private set; }

        public Task PublishResponseAsync(ResponseMessage message, CancellationToken ct = default)
        {
            Published = message;
            return Task.CompletedTask;
        }
    }

    private sealed class FixedClock : IDateTimeProvider
    {
        public FixedClock(DateTime now) => UtcNow = now;
        public DateTime UtcNow { get; }
    }
}
