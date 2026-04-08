using Microsoft.AspNetCore.Mvc;

namespace ScrumExtreme.Web.Controllers;

[Route("MainPage")]
public class MainPageController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
}
