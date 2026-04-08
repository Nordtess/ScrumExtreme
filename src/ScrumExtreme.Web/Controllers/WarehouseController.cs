using Microsoft.AspNetCore.Mvc;

namespace ScrumExtreme.Web.Controllers;

[Route("Warehouse")]
public class WarehouseController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
}
