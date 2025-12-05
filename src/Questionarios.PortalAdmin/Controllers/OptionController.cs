using Microsoft.AspNetCore.Mvc;
using Questionarios.PortalAdmin.Models;
using Questionarios.PortalAdmin.Services;

namespace Questionarios.PortalAdmin.Controllers;

public class OptionController : Controller
{
    private readonly IQuestionariosApiClient _api;

    public OptionController(IQuestionariosApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid? surveyId, Guid? questionId, CancellationToken ct)
    {
        var surveys = await _api.GetSurveysAsync(ct);
        var selectedSurveyId = surveyId ?? surveys.FirstOrDefault()?.Id;
        var questions = selectedSurveyId.HasValue
            ? await _api.GetQuestionsBySurveyAsync(selectedSurveyId.Value, ct)
            : Array.Empty<QuestionDetailDto>();

        var selectedQuestionId = questionId ?? questions.FirstOrDefault()?.Id;
        var options = selectedQuestionId.HasValue
            ? questions.FirstOrDefault(q => q.Id == selectedQuestionId.Value)?.Options ?? new List<OptionDetailDto>()
            : new List<OptionDetailDto>();

        var vm = new OptionListViewModel
        {
            Surveys = surveys.ToList(),
            SelectedSurveyId = selectedSurveyId,
            Questions = questions.ToList(),
            SelectedQuestionId = selectedQuestionId,
            Options = options.ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OptionCreateInput input, Guid surveyId, CancellationToken ct)
    {
        if (input.QuestionId == Guid.Empty || string.IsNullOrWhiteSpace(input.Text))
        {
            TempData["Error"] = "Selecione uma pergunta e preencha o texto da opção.";
            return RedirectToAction(nameof(Index), new { surveyId, questionId = input.QuestionId });
        }

        await _api.CreateOptionAsync(input, ct);
        TempData["Success"] = "Opção criada.";
        return RedirectToAction(nameof(Index), new { surveyId, questionId = input.QuestionId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid surveyId, Guid questionId, CancellationToken ct)
    {
        await _api.DeleteOptionAsync(id, ct);
        TempData["Success"] = "Opção removida.";
        return RedirectToAction(nameof(Index), new { surveyId, questionId });
    }
}
