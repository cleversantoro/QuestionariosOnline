using Questionarios.Application.Abstractions;
using Questionarios.Application.DTOs;
using Questionarios.Domain.Abstractions;

namespace Questionarios.Application.Services;

public class ResponseService : IResponseService
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IQueueClient _queue;
    private readonly IDateTimeProvider _clock;

    public ResponseService(
        ISurveyRepository surveyRepository,
        IQueueClient queue,
        IDateTimeProvider clock)
    {
        _surveyRepository = surveyRepository;
        _queue = queue;
        _clock = clock;
    }

    public async Task SubmitAsync(ResponseCreateDto dto, CancellationToken ct = default)
    {
        var survey = await _surveyRepository.GetByIdAsync(dto.SurveyId, ct)
                     ?? throw new InvalidOperationException("Survey not found");

        if (!survey.IsActive(_clock.UtcNow))
            throw new InvalidOperationException("Survey is not active.");

        var msg = new ResponseMessage(
            dto.SurveyId,
            dto.Items.Select(i => (i.QuestionId, i.OptionId)).ToList()
        );

        await _queue.PublishResponseAsync(msg, ct);
    }
}
