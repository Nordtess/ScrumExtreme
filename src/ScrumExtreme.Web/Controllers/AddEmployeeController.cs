using Microsoft.AspNetCore.Mvc;

namespace ScrumExtreme.Web.Controllers;

[Route("AddEmployee")]
public class AddEmployeeController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
}
