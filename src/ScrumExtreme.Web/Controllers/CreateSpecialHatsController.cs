using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Web.Models;

namespace ScrumExtreme.Web.Controllers
{
    public class CreateSpecialHatsController : Controller
    {
        public IActionResult Index()
        {
            return View(new CreateHatsViewModel());
        }
    }
}
