using Microsoft.AspNetCore.Mvc;

namespace ScrumExtreme.Web.Controllers;

[Route("Statistics")]
public class SalesStatisticsController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
}
