using System.Net;
using System.Net.Http.Json;
using Questionarios.Application.DTOs;

namespace Questionarios.WebPublico.Services;

public interface ISurveyApiClient
{
    Task<SurveyDetailDto?> GetSurveyAsync(Guid surveyId, CancellationToken ct = default);
    Task<bool> SubmitResponseAsync(Guid surveyId, IList<ResponseItemDto> items, CancellationToken ct = default);
}

public class SurveyApiClient : ISurveyApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SurveyApiClient> _logger;

    public SurveyApiClient(HttpClient httpClient, ILogger<SurveyApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<SurveyDetailDto?> GetSurveyAsync(Guid surveyId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"/api/survey/{surveyId}", ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Erro ao obter pesquisa {SurveyId}: {Status}", surveyId, response.StatusCode);
            return null;
        }

        return await response.Content.ReadFromJsonAsync<SurveyDetailDto>(cancellationToken: ct);
    }

    public async Task<bool> SubmitResponseAsync(Guid surveyId, IList<ResponseItemDto> items, CancellationToken ct = default)
    {
        var payload = new ResponseCreateDto(surveyId, items);
        var response = await _httpClient.PostAsJsonAsync("/api/responses", payload, ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Erro ao enviar respostas da pesquisa {SurveyId}: {Status}", surveyId, response.StatusCode);
        }

        return response.IsSuccessStatusCode;
    }
}
