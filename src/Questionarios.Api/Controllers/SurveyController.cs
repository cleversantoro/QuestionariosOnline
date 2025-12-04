using Microsoft.AspNetCore.Mvc;
using Questionarios.Application.DTOs;
using Questionarios.Application.Services;

namespace Questionarios.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SurveyController : ControllerBase
{
    private readonly ISurveyService _surveyService;

    public SurveyController(ISurveyService surveyService)
    {
        _surveyService = surveyService;
    }

    // AGORA RETORNA DETALHE (com perguntas + opções)
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var survey = await _surveyService.GetDetailAsync(id, ct);
        return Ok(survey);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SurveyCreateDto dto, CancellationToken ct)
    {
        var survey = await _surveyService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id = survey.Id }, survey);
    }

    [HttpPost("{id:guid}/close")]
    public async Task<IActionResult> Close(Guid id, CancellationToken ct)
    {
        await _surveyService.CloseAsync(id, ct);
        return NoContent();
    }
}
