using Questionarios.Application.DTOs;
using Questionarios.Domain.Abstractions;
using Questionarios.Domain.Entities;

namespace Questionarios.Application.Services;

public class SurveyService : ISurveyService
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IDateTimeProvider _clock;

    public SurveyService(ISurveyRepository surveyRepository, IDateTimeProvider clock)
    {
        _surveyRepository = surveyRepository;
        _clock = clock;
    }

    public async Task<SurveySummaryDto> CreateAsync(SurveyCreateDto dto, CancellationToken ct = default)
    {
        var survey = new Survey(dto.Title, dto.StartAt, dto.EndAt);
        await _surveyRepository.AddAsync(survey, ct);

        return MapToSummary(survey);
    }

    public async Task<SurveySummaryDto> GetAsync(Guid id, CancellationToken ct = default)
    {
        var survey = await _surveyRepository.GetByIdAsync(id, ct)
                     ?? throw new InvalidOperationException("Survey not found");

        return MapToSummary(survey);
    }

    public async Task<SurveyDetailDto> GetDetailAsync(Guid id, CancellationToken ct = default)
    {
        var survey = await _surveyRepository.GetByIdAsync(id, ct)
                     ?? throw new InvalidOperationException("Survey not found");

        return MapToDetail(survey);
    }

    public async Task CloseAsync(Guid id, CancellationToken ct = default)
    {
        var survey = await _surveyRepository.GetByIdAsync(id, ct)
                     ?? throw new InvalidOperationException("Survey not found");

        survey.Close();
        await _surveyRepository.UpdateAsync(survey, ct);
    }

    private static SurveySummaryDto MapToSummary(Survey survey) =>
        new(
            survey.Id,
            survey.Title,
            survey.StartAt,
            survey.EndAt,
            survey.IsClosed
        );

    private static SurveyDetailDto MapToDetail(Survey survey)
    {
        var questions = survey.Questions
            .OrderBy(q => q.Order ?? int.MaxValue)
            .Select(q => new QuestionDetailDto(
                q.Id,
                q.Text,
                q.Order,
                q.Options
                    .OrderBy(o => o.Order ?? int.MaxValue)
                    .Select(o => new OptionDetailDto(o.Id, o.Text, o.Order))
                    .ToList()
            ))
            .ToList();

        return new SurveyDetailDto(
            survey.Id,
            survey.Title,
            survey.StartAt,
            survey.EndAt,
            survey.IsClosed,
            questions
        );
    }
}
