using Microsoft.AspNetCore.Mvc;

namespace ScrumExtreme.Web.Controllers;

[Route("Orders")]
public class OrdersController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
}
