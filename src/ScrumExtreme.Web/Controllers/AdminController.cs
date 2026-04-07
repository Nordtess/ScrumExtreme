using Microsoft.AspNetCore.Mvc;

namespace ScrumExtreme.Web.Controllers;

public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
