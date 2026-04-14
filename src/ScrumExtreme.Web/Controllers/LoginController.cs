using Microsoft.AspNetCore.Mvc;

namespace ScrumExtreme.Web.Controllers;

[Route("Login")]
public class LoginController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View("Login");
    }

    public IActionResult CreateAccount()
    {
        return View();
    }
}
