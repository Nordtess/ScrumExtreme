using Microsoft.AspNetCore.Mvc;

namespace ScrumExtreme.Web.Controllers;

[Route("Huvudsida")]
public class MainPageController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
}
