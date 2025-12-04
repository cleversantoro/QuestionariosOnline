using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Questionarios.PortalAdmin.Models;

namespace Questionarios.PortalAdmin.Controllers;

public class AuthController : Controller
{
    private const string SessionUserNameKey = "UserName";
    private static readonly List<MockUser> MockUsers =
    [
        new("admin@questionarios.com", "Admin@123", "Admin Portal"),
        new("gestor@questionarios.com", "Gestor@123", "Gestor")
    ];

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetString(SessionUserNameKey) is not null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = MockUsers.FirstOrDefault(u =>
            string.Equals(u.Email, model.Email.Trim(), StringComparison.OrdinalIgnoreCase)
            && u.Password == model.Password);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos. Use o mock abaixo.");
            return View(model);
        }

        HttpContext.Session.SetString(SessionUserNameKey, user.Name);
        HttpContext.Session.SetString("PreferredLanguage", model.Language ?? "pt");

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }
}
