using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Web.Models;

namespace ScrumExtreme.Web.Controllers
{
    public class CreateHatsController : Controller
    {
        private readonly IHatService _hatService;
        private readonly IMaterialService _materialService;

        public CreateHatsController(IHatService hatService, IMaterialService materialService)
        {
            _hatService = hatService;
            _materialService = materialService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var materials = await _materialService.GetMaterialsAsync();
            return View(new CreateHatsViewModel
            {
                AvailableMaterials = materials.Select(m => m.Name).OrderBy(n => n).ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateHats(CreateHatsViewModel model)
        {
            if (model.Sizes == null || !model.Sizes.Any())
                ModelState.AddModelError(nameof(model.Sizes), "Minst en storlek måste väljas.");

            if (model.SelectedMaterials == null || !model.SelectedMaterials.Any())
                ModelState.AddModelError(nameof(model.SelectedMaterials), "Minst ett material måste väljas.");

            ModelState.Remove(nameof(model.AvailableMaterials));

            if (!ModelState.IsValid)
            {
                var mats = await _materialService.GetMaterialsAsync();
                model.AvailableMaterials = mats.Select(m => m.Name).OrderBy(n => n).ToList();
                return View("Index", model);
            }

            var existing = await _hatService.GetAllHatsAsync();
            if (existing.Any(h => h.Name.Equals(model.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError(nameof(model.Name), $"\"{model.Name}\" finns redan i systemet.");
                var mats = await _materialService.GetMaterialsAsync();
                model.AvailableMaterials = mats.Select(m => m.Name).OrderBy(n => n).ToList();
                return View("Index", model);
            }

            var textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
            var hat = new Hats
            {
                Name = textInfo.ToTitleCase(model.Name.Trim().ToLower()),
                Sizes = model.Sizes ?? new List<string>(),
                Price = model.Price,
                MaterialList = string.Join(", ", (model.SelectedMaterials ?? new List<string>()).Select(m => m.Trim())),
            };

            await _hatService.CreateHatsAsync(hat);
            TempData["Success"] = "Ny hatt skapades!";
            return RedirectToAction(nameof(Index));
        }

    }

}
