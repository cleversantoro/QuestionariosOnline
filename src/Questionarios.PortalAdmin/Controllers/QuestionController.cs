using Microsoft.AspNetCore.Mvc;
using Questionarios.PortalAdmin.Models;
using Questionarios.PortalAdmin.Services;

namespace Questionarios.PortalAdmin.Controllers;

public class QuestionController : Controller
{
    private readonly IQuestionariosApiClient _api;

    public QuestionController(IQuestionariosApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid? surveyId, CancellationToken ct)
    {
        var surveys = await _api.GetSurveysAsync(ct);
        var selectedId = surveyId ?? surveys.FirstOrDefault()?.Id;
        var questions = selectedId.HasValue
            ? await _api.GetQuestionsBySurveyAsync(selectedId.Value, ct)
            : Array.Empty<QuestionDetailDto>();

        var vm = new QuestionListViewModel
        {
            Surveys = surveys.ToList(),
            SelectedSurveyId = selectedId,
            Questions = questions.ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(QuestionCreateInput input, CancellationToken ct)
    {
        if (input.SurveyId == Guid.Empty || string.IsNullOrWhiteSpace(input.Text))
        {
            TempData["Error"] = "Selecione um questionário e informe o texto da pergunta.";
            return RedirectToAction(nameof(Index), new { surveyId = input.SurveyId });
        }

        await _api.CreateQuestionAsync(input, ct);
        TempData["Success"] = "Pergunta criada.";
        return RedirectToAction(nameof(Index), new { surveyId = input.SurveyId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid surveyId, CancellationToken ct)
    {
        await _api.DeleteQuestionAsync(id, ct);
        TempData["Success"] = "Pergunta removida.";
        return RedirectToAction(nameof(Index), new { surveyId });
    }
}
