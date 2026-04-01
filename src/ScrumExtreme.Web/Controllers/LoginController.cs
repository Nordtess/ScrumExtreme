using Microsoft.AspNetCore.Mvc;

namespace ScrumExtreme.Web.Controllers;

public class LoginController : Controller
{
    public IActionResult Login()
    {
        return View();
    }
}
