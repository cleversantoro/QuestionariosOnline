using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Questionarios.PortalAdmin.Models;
using Questionarios.PortalAdmin.Services;

namespace Questionarios.PortalAdmin.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IQuestionariosApiClient _api;
    private readonly IConfiguration _configuration;
    private const string SessionUserNameKey = "UserName";

    public HomeController(ILogger<HomeController> logger, IQuestionariosApiClient api, IConfiguration configuration)
    {
        _logger = logger;
        _api = api;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var userName = HttpContext.Session.GetString(SessionUserNameKey);
        if (string.IsNullOrWhiteSpace(userName))
        {
            return RedirectToAction("Login", "Auth");
        }

        var surveys = await _api.GetSurveysAsync(ct);
        var selectedSurvey = surveys.FirstOrDefault();
        SurveyChartResultDto? chart = null;
        if (selectedSurvey is not null)
        {
            chart = await _api.GetSurveyChartAsync(selectedSurvey.Id, ct);
        }

        var viewModel = new DashboardViewModel
        {
            UserName = userName,
            SelectedSurveyId = selectedSurvey?.Id,
            Surveys = surveys.ToList(),
            ChartData = chart,
            ApiBaseUrl = _configuration.GetSection(Services.ApiOptions.SectionName).GetValue<string>("BaseUrl") ?? string.Empty,
            Highlights = new List<HighlightCard>
            {
                new("Questionários", surveys.Count.ToString(), "+", true),
                new("Respostas", chart?.Questions.Sum(q => q.Options.Sum(o => o.Votes)).ToString() ?? "0", "+", true),
                new("Questões", chart?.Questions.Count.ToString() ?? "0", "+", true),
                new("Ativos", surveys.Count(s => !s.IsClosed).ToString(), "Ativos", true)
            },
            Activities = new List<ActivityItem>
            {
                new("Questionário", "Carregado do serviço", "Sync"),
                new("Resultados", "Gráficos baseados na API", "Dados"),
                new("Usuário", $"Logado como {userName}", "Sessão")
            }
        };

        return View(viewModel);
    }

    public IActionResult Profile()
    {
        var userName = HttpContext.Session.GetString(SessionUserNameKey);
        if (string.IsNullOrWhiteSpace(userName))
        {
            return RedirectToAction("Login", "Auth");
        }

        var model = new ProfileViewModel
        {
            UserName = userName,
            Email = $"{userName}@questionarios.com",
            Role = "Administrador",
            Language = HttpContext.Session.GetString("PreferredLanguage") ?? "pt"
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
