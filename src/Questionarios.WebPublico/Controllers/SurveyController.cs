using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Questionarios.Application.DTOs;
using Questionarios.WebPublico.Models;
using Questionarios.WebPublico.Services;

namespace Questionarios.WebPublico.Controllers;

public class SurveyController : Controller
{
    private readonly ISurveyApiClient _surveyApiClient;
    private readonly ApiSettings _apiSettings;

    public SurveyController(ISurveyApiClient surveyApiClient, IOptions<ApiSettings> apiSettings)
    {
        _surveyApiClient = surveyApiClient;
        _apiSettings = apiSettings.Value;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid? id, CancellationToken ct)
    {
        var surveyId = id ?? _apiSettings.DefaultSurveyId;
        if (surveyId is null)
        {
            return View(new SurveyResponseViewModel
            {
                ErrorMessage = "Nenhuma pesquisa configurada."
            });
        }

        var survey = await _surveyApiClient.GetSurveyAsync(surveyId.Value, ct);
        if (survey is null)
        {
            return View(new SurveyResponseViewModel
            {
                ErrorMessage = "Pesquisa não encontrada ou indisponível no momento."
            });
        }

        var answers = survey.Questions
            .OrderBy(q => q.Order)
            .Select(q => new QuestionAnswerInput
            {
                QuestionId = q.Id,
                QuestionText = q.Text
            })
            .ToList();

        return View(new SurveyResponseViewModel
        {
            Survey = survey,
            Answers = answers
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(Guid surveyId, SurveyResponseViewModel model, CancellationToken ct)
    {
        var survey = await _surveyApiClient.GetSurveyAsync(surveyId, ct);
        if (survey is null)
        {
            model.ErrorMessage = "Pesquisa não encontrada.";
            return View("Index", model);
        }

        model.Survey = survey;

        var orderLookup = survey.Questions
            .Select((q, index) => new { q.Id, index })
            .ToDictionary(x => x.Id, x => x.index);

        model.Answers = model.Answers
            .OrderBy(a => orderLookup.GetValueOrDefault(a.QuestionId, int.MaxValue))
            .ToList();

        if (model.Answers.Any(a => a.SelectedOptionId is null))
        {
            ModelState.AddModelError(string.Empty, "Responda todas as perguntas antes de enviar.");
        }

        if (model.SendCopy && string.IsNullOrWhiteSpace(model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "Informe um e-mail para receber uma cópia.");
        }

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var items = model.Answers
            .Select(a => new ResponseItemDto(a.QuestionId, a.SelectedOptionId!.Value))
            .ToList();

        var ok = await _surveyApiClient.SubmitResponseAsync(surveyId, items, ct);

        if (!ok)
        {
            model.ErrorMessage = "Não foi possível salvar sua resposta. Tente novamente em instantes.";
            return View("Index", model);
        }

        model.SuccessMessage = model.SendCopy && !string.IsNullOrWhiteSpace(model.Email)
            ? "Respostas enviadas! Em breve você receberá uma cópia no e-mail informado."
            : "Respostas enviadas! Obrigado por participar.";

        // Limpa seleções para novo envio se desejar
        foreach (var answer in model.Answers)
        {
            answer.SelectedOptionId = null;
        }
        model.SendCopy = false;
        model.Email = null;

        ModelState.Clear();
        return View("Index", model);
    }
}
