using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;

namespace ScrumExtreme.Web.Controllers;

[Route("Login")]
public class LoginController : Controller
{
    private readonly IUserService _userService;

    public LoginController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        // Already logged in → redirect based on role
        var role = HttpContext.Session.GetString("UserRole");
        if (!string.IsNullOrEmpty(role))
            return RedirectToAction("Index", "MainPage");

        return View("Login");
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([FromForm] string username, [FromForm] string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Fyll i användarnamn och lösenord.";
            return View("Login");
        }

        var user = await _userService.GetByUsernameAsync(username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            ViewBag.Error = "Felaktigt användarnamn eller lösenord.";
            return View("Login");
        }

        HttpContext.Session.SetString("UserId", user.Id.ToString());
        HttpContext.Session.SetString("UserRole", user.Role);
        HttpContext.Session.SetString("UserFullName", $"{user.FirstName} {user.LastName}");

        return RedirectToAction("Index", "MainPage");
    }

    [HttpGet("Logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Login");
    }
}

