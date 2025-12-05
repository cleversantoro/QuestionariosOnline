using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Questionarios.PortalAdmin.Models;
using Questionarios.PortalAdmin.Services;

namespace Questionarios.PortalAdmin.Controllers;

public class AuthController : Controller
{
    private const string SessionUserNameKey = "UserName";
    private readonly IQuestionariosApiClient _api;

    public AuthController(IQuestionariosApiClient api)
    {
        _api = api;
    }

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
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _api.LoginAsync(model.Email.Trim(), model.Password, HttpContext.RequestAborted);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
            return View(model);
        }

        HttpContext.Session.SetString(SessionUserNameKey, user.Name);
        HttpContext.Session.SetString("PreferredLanguage", model.Language ?? "pt");
        HttpContext.Session.SetString("UserEmail", user.Email);

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
