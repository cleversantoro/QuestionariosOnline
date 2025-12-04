using Microsoft.AspNetCore.Mvc;
using Questionarios.Application.Services;

namespace Questionarios.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResultsController : ControllerBase
{
    private readonly IResultsService _resultsService;

    public ResultsController(IResultsService resultsService)
    {
        _resultsService = resultsService;
    }

    [HttpGet("{surveyId:guid}")]
    public async Task<IActionResult> Get(Guid surveyId, CancellationToken ct)
    {
        var result = await _resultsService.GetAggregatedAsync(surveyId, ct);
        return Ok(result);
    }

    // NOVO: formato pronto pra gráfico
    [HttpGet("{surveyId:guid}/chart")]
    public async Task<IActionResult> GetChart(Guid surveyId, CancellationToken ct)
    {
        var chart = await _resultsService.GetChartAsync(surveyId, ct);
        return Ok(chart);
    }
}
