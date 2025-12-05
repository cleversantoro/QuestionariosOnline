using Microsoft.AspNetCore.Mvc;
using Questionarios.Application.DTOs;
using Questionarios.Application.Services;

namespace Questionarios.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionsController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] QuestionCreateDto dto, CancellationToken ct)
    {
        await _questionService.AddQuestionAsync(dto, ct);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var question = await _questionService.GetAsync(id, ct);
        return Ok(question);
    }

    [HttpGet("survey/{surveyId:guid}")]
    public async Task<IActionResult> GetBySurvey(Guid surveyId, CancellationToken ct)
    {
        var questions = await _questionService.GetBySurveyAsync(surveyId, ct);
        return Ok(questions);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] QuestionCreateDto dto, CancellationToken ct)
    {
        await _questionService.UpdateQuestionAsync(id, dto.Text, dto.Order, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _questionService.DeleteQuestionAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{questionId:guid}/options")]
    public async Task<IActionResult> CreateOption(Guid questionId, [FromBody] OptionCreateDto dto, CancellationToken ct)
    {
        dto = dto with { QuestionId = questionId };
        await _questionService.AddOptionAsync(dto, ct);
        return NoContent();
    }

    [HttpPut("options/{id:guid}")]
    public async Task<IActionResult> UpdateOption(Guid id, [FromBody] OptionCreateDto dto, CancellationToken ct)
    {
        await _questionService.UpdateOptionAsync(id, dto.Text, dto.Order, ct);
        return NoContent();
    }

    [HttpDelete("options/{id:guid}")]
    public async Task<IActionResult> DeleteOption(Guid id, CancellationToken ct)
    {
        await _questionService.DeleteOptionAsync(id, ct);
        return NoContent();
    }
}
