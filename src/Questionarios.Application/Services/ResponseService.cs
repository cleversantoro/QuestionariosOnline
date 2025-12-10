using Questionarios.Application.Abstractions;
using Questionarios.Application.DTOs;
using Questionarios.Domain.Abstractions;
using Questionarios.Domain.Entities;

namespace Questionarios.Application.Services;

public class ResponseService : IResponseService
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IResponseRepository _responseRepository;
    private readonly IResultsRepository _resultsRepository;
    private readonly IDateTimeProvider _clock;

    public ResponseService(
        ISurveyRepository surveyRepository,
        IResponseRepository responseRepository,
        IResultsRepository resultsRepository,
        IDateTimeProvider clock)
    {
        _surveyRepository = surveyRepository;
        _responseRepository = responseRepository;
        _resultsRepository = resultsRepository;
        _clock = clock;
    }

    public async Task<ResponseDto> SubmitAsync(ResponseCreateDto dto, CancellationToken ct = default)
    {
        var survey = await _surveyRepository.GetByIdAsync(dto.SurveyId, ct)
                     ?? throw new InvalidOperationException("Survey not found");

        if (!survey.IsActive(_clock.UtcNow))
            throw new InvalidOperationException("Survey is not active.");

        if (dto.Items is null || dto.Items.Count == 0)
            throw new InvalidOperationException("At least one answer is required.");

        var response = new Response(dto.SurveyId, _clock.UtcNow);

        foreach (var item in dto.Items)
        {
            var question = survey.Questions.FirstOrDefault(q => q.Id == item.QuestionId)
                           ?? throw new InvalidOperationException("Question does not belong to this survey.");

            var option = question.Options.FirstOrDefault(o => o.Id == item.OptionId)
                         ?? throw new InvalidOperationException("Option does not belong to the selected question.");

            response.AddItem(question.Id, option.Id);

            var aggregated = new AggregatedResult(dto.SurveyId, question.Id, option.Id);
            await _resultsRepository.UpsertAsync(aggregated, ct);
        }

        await _responseRepository.AddAsync(response, ct);

        return new ResponseDto(response.Id, response.SurveyId, response.AnsweredAt);
    }
}
