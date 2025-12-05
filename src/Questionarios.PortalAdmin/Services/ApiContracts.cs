using System.Net.Http.Json;
using Questionarios.PortalAdmin.Models;

namespace Questionarios.PortalAdmin.Services;

public interface IQuestionariosApiClient
{
    Task<UserInfo?> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<IReadOnlyList<SurveySummaryDto>> GetSurveysAsync(CancellationToken ct = default);
    Task<SurveyChartResultDto?> GetSurveyChartAsync(Guid surveyId, CancellationToken ct = default);
    Task<SurveySummaryDto?> CreateSurveyAsync(SurveyCreateInput input, CancellationToken ct = default);
    Task DeleteSurveyAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<QuestionDetailDto>> GetQuestionsBySurveyAsync(Guid surveyId, CancellationToken ct = default);
    Task CreateQuestionAsync(QuestionCreateInput input, CancellationToken ct = default);
    Task DeleteQuestionAsync(Guid id, CancellationToken ct = default);
    Task CreateOptionAsync(OptionCreateInput input, CancellationToken ct = default);
    Task DeleteOptionAsync(Guid id, CancellationToken ct = default);
}

public class QuestionariosApiClient : IQuestionariosApiClient
{
    private readonly HttpClient _http;

    public QuestionariosApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<UserInfo?> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var payload = new { Email = email, Password = password };
        var response = await _http.PostAsJsonAsync("api/users/login", payload, ct);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<UserInfo>(cancellationToken: ct);
    }

    public async Task<IReadOnlyList<SurveySummaryDto>> GetSurveysAsync(CancellationToken ct = default)
    {
        var data = await _http.GetFromJsonAsync<IReadOnlyList<SurveySummaryDto>>("api/survey", cancellationToken: ct);
        return data ?? Array.Empty<SurveySummaryDto>();
    }

    public Task<SurveyChartResultDto?> GetSurveyChartAsync(Guid surveyId, CancellationToken ct = default) =>
        _http.GetFromJsonAsync<SurveyChartResultDto>($"api/results/{surveyId}/chart", cancellationToken: ct);

    public async Task<SurveySummaryDto?> CreateSurveyAsync(SurveyCreateInput input, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/survey", input, ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<SurveySummaryDto>(cancellationToken: ct);
    }

    public async Task DeleteSurveyAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"api/survey/{id}", ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyList<QuestionDetailDto>> GetQuestionsBySurveyAsync(Guid surveyId, CancellationToken ct = default)
    {
        var data = await _http.GetFromJsonAsync<IReadOnlyList<QuestionDetailDto>>($"api/questions/survey/{surveyId}", cancellationToken: ct);
        return data ?? Array.Empty<QuestionDetailDto>();
    }

    public async Task CreateQuestionAsync(QuestionCreateInput input, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/questions", input, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteQuestionAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"api/questions/{id}", ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task CreateOptionAsync(OptionCreateInput input, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync($"api/questions/{input.QuestionId}/options", input, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteOptionAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"api/questions/options/{id}", ct);
        response.EnsureSuccessStatusCode();
    }
}
