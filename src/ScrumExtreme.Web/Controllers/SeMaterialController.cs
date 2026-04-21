using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;

namespace ScrumExtreme.Web.Controllers
{
    public class SeMaterialController : Controller
    {
        private readonly IMaterialService _materialService;

        public SeMaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search)
        {
            var materials = await _materialService.GetAllMaterialsAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                materials = materials.Where(m =>
                    !string.IsNullOrWhiteSpace(m.Name) &&
                    m.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            return View(materials);
        }
    }
}