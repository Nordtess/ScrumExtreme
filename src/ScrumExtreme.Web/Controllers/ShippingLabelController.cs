using Microsoft.AspNetCore.Mvc;

namespace ScrumExtreme.Web.Controllers;

[Route("ShippingLabel")]
public class ShippingLabelController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
}
