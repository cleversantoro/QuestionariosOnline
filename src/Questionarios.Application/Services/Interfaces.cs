using Questionarios.Application.DTOs;

namespace Questionarios.Application.Services;


public interface ISurveyService
{
    Task<SurveySummaryDto> CreateAsync(SurveyCreateDto dto, CancellationToken ct = default);
    Task<SurveySummaryDto> GetAsync(Guid id, CancellationToken ct = default);
    Task<SurveyDetailDto> GetDetailAsync(Guid id, CancellationToken ct = default);
    Task CloseAsync(Guid id, CancellationToken ct = default);
}

public interface IQuestionService
{
    Task AddQuestionAsync(QuestionCreateDto dto, CancellationToken ct = default);
    Task AddOptionAsync(OptionCreateDto dto, CancellationToken ct = default);
}

public interface IResponseService
{
    Task SubmitAsync(ResponseCreateDto dto, CancellationToken ct = default);
}

public interface IResultsService
{
    Task<AggregatedResultDto> GetAggregatedAsync(Guid surveyId, CancellationToken ct = default);
    Task<SurveyChartResultDto> GetChartAsync(Guid surveyId, CancellationToken ct = default);

}
