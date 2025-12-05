using Microsoft.AspNetCore.Mvc;
using Questionarios.PortalAdmin.Models;
using Questionarios.PortalAdmin.Services;

namespace Questionarios.PortalAdmin.Controllers;

public class SurveyController : Controller
{
    private readonly IQuestionariosApiClient _api;

    public SurveyController(IQuestionariosApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var surveys = await _api.GetSurveysAsync(ct);
        return View(new SurveyListViewModel { Surveys = surveys.ToList() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SurveyCreateInput input, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(input.Title))
        {
            TempData["Error"] = "Titulo obrigatorio.";
            return RedirectToAction(nameof(Index));
        }

        var created = await _api.CreateSurveyAsync(input, ct);
        if (created is null)
        {
            TempData["Error"] = "Nao foi possivel criar o questionario.";
        }
        else
        {
            TempData["Success"] = "Questionario criado com sucesso.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _api.DeleteSurveyAsync(id, ct);
        TempData["Success"] = "Questionario excluido.";
        return RedirectToAction(nameof(Index));
    }
}
