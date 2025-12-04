using Questionarios.Application.Abstractions;
using Questionarios.Application.DTOs;
using Questionarios.Domain.Abstractions;

namespace Questionarios.Application.Services;

public class ResultsService : IResultsService
{
    private readonly IResultsRepository _resultsRepository;
    private readonly ISurveyRepository _surveyRepository;
    private readonly ICacheService _cache;

    public ResultsService(
        IResultsRepository resultsRepository,
        ISurveyRepository surveyRepository,
        ICacheService cache)
    {
        _resultsRepository = resultsRepository;
        _surveyRepository = surveyRepository;
        _cache = cache;
    }

    public async Task<AggregatedResultDto> GetAggregatedAsync(Guid surveyId, CancellationToken ct = default)
    {
        // Se você não estiver usando mais AggregatedResult, pode até remover isso
        var result = await _resultsRepository.GetBySurveyIdAsync(surveyId, ct)
                     ?? throw new InvalidOperationException("Aggregated result not found.");

        return new AggregatedResultDto(result.SurveyId, result.QuestionId, result.OptionId ,result.Votes);
    }

    public async Task<SurveyChartResultDto> GetChartAsync(Guid surveyId, CancellationToken ct = default)
    {
        var cacheKey = $"chart:{surveyId}";

        var cached = await _cache.GetAsync<SurveyChartResultDto>(cacheKey, ct);
        if (cached is not null)
            return cached;

        var survey = await _surveyRepository.GetByIdAsync(surveyId, ct)
                     ?? throw new InvalidOperationException("Survey not found.");

        var optionVotes = await _resultsRepository.GetOptionVotesAsync(surveyId, ct);
        if (!optionVotes.Any())
            throw new InvalidOperationException("No responses found for this survey.");

        var questions = optionVotes
            .GroupBy(ov => new { ov.QuestionId, ov.QuestionText })
            .Select(g =>
            {
                var totalVotes = g.Sum(x => x.Votes);

                var options = g
                    .Select(x =>
                        new ChartOptionDto(
                            x.OptionId,
                            x.OptionText,
                            x.Votes,
                            totalVotes == 0 ? 0 : (double)x.Votes * 100.0 / totalVotes
                        ))
                    .ToList();

                return new ChartQuestionDto(
                    g.Key.QuestionId,
                    g.Key.QuestionText,
                    options
                );
            })
            .ToList();

        var dto = new SurveyChartResultDto(
            survey.Id,
            survey.Title,
            questions
        );

        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(1), ct);

        return dto;
    }
}
