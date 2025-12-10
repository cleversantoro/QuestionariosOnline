using Questionarios.Application.DTOs;

namespace Questionarios.Application.Services;


public interface ISurveyService
{
    Task<SurveySummaryDto> CreateAsync(SurveyCreateDto dto, CancellationToken ct = default);
    Task<SurveySummaryDto> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<SurveySummaryDto>> GetAllAsync(CancellationToken ct = default);
    Task<SurveyDetailDto> GetDetailAsync(Guid id, CancellationToken ct = default);
    Task UpdateAsync(Guid id, SurveyCreateDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task CloseAsync(Guid id, CancellationToken ct = default);
}

public interface IQuestionService
{
    Task AddQuestionAsync(QuestionCreateDto dto, CancellationToken ct = default);
    Task<QuestionDetailDto> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<QuestionDetailDto>> GetBySurveyAsync(Guid surveyId, CancellationToken ct = default);
    Task AddOptionAsync(OptionCreateDto dto, CancellationToken ct = default);
    Task UpdateQuestionAsync(Guid id, string text, int? order, CancellationToken ct = default);
    Task DeleteQuestionAsync(Guid id, CancellationToken ct = default);
    Task UpdateOptionAsync(Guid id, string text, int? order, CancellationToken ct = default);
    Task DeleteOptionAsync(Guid id, CancellationToken ct = default);
}

public interface IResponseService
{
    Task<ResponseDto> SubmitAsync(ResponseCreateDto dto, CancellationToken ct = default);
}

public interface IResultsService
{
    Task<AggregatedResultDto> GetAggregatedAsync(Guid surveyId, CancellationToken ct = default);
    Task<SurveyChartResultDto> GetChartAsync(Guid surveyId, CancellationToken ct = default);

}

public interface IUserService
{
    Task<UserDto> CreateAsync(UserCreateDto dto, CancellationToken ct = default);
    Task<UserDto?> AuthenticateAsync(UserLoginDto dto, CancellationToken ct = default);
    Task<UserDto> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default);
    Task UpdateAsync(Guid id, UserUpdateDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
