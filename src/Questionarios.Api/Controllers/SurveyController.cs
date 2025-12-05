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

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var surveys = await _surveyService.GetAllAsync(ct);
        return Ok(surveys);
    }

    // AGORA RETORNA DETALHE (com perguntas + opcoes)
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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SurveyCreateDto dto, CancellationToken ct)
    {
        await _surveyService.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _surveyService.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/close")]
    public async Task<IActionResult> Close(Guid id, CancellationToken ct)
    {
        await _surveyService.CloseAsync(id, ct);
        return NoContent();
    }
}
