using Microsoft.AspNetCore.Mvc;

namespace ScrumExtreme.Web.Controllers;

public class LoginController : Controller
{
    public IActionResult Index()
    {
        return View("Login");
    }

    public IActionResult CreateAccount()
    {
        return View();
    }
}
