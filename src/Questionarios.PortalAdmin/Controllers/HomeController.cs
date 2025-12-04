using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Questionarios.PortalAdmin.Models;

namespace Questionarios.PortalAdmin.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private const string SessionUserNameKey = "UserName";

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var userName = HttpContext.Session.GetString(SessionUserNameKey);
        if (string.IsNullOrWhiteSpace(userName))
        {
            return RedirectToAction("Login", "Auth");
        }

        var questionarios = new List<string>
        {
            "Pesquisa de Satisfação",
            "Clima Organizacional",
            "Onboarding",
            "NPS Produtos"
        };

        var viewModel = new DashboardViewModel
        {
            UserName = userName,
            SelectedQuestionario = questionarios.First(),
            Questionarios = questionarios,
            Highlights = new List<HighlightCard>
            {
                new("Taxa de Resposta", "82%", "+4.3%", true),
                new("Tempo Médio", "3m 12s", "-0.8%", true),
                new("Pontuação NPS", "64", "+2", true),
                new("Questionários Ativos", "12", "Novo", true)
            },
            Activities = new List<ActivityItem>
            {
                new("Questionário publicado", "Onboarding 2025 foi publicado para 430 usuários", "Publicado"),
                new("Nova pergunta", "Bloco de métricas adicionado em Pesquisa de Satisfação", "Revisão"),
                new("Alerta", "Taxa de abandono alta no bloco final de Clima", "Alerta")
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
