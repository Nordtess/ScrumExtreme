using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Web.Models;

namespace ScrumExtreme.Web.Controllers
{
    public class CreateHatsController : Controller
    {
        private readonly IHatService _hatService;

        public CreateHatsController(IHatService hatService)
        {
            _hatService = hatService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new CreateHatsViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateHats(CreateHatsViewModel model)
        {
            if (model.Sizes == null || !model.Sizes.Any())
                ModelState.AddModelError(nameof(model.Sizes), "Minst en storlek måste väljas.");

            if (!ModelState.IsValid)
                return View("Index", model);

            // Duplicate name check
            var existing = await _hatService.GetAllHatsAsync();
            if (existing.Any(h => h.Name.Equals(model.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError(nameof(model.Name), $"\"{model.Name}\" finns redan i systemet.");
                return View("Index", model);
            }

            var textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
            var hat = new Hats
            {
                Name = textInfo.ToTitleCase(model.Name.Trim().ToLower()),
                Sizes = model.Sizes,
                Price = model.Price,
                MaterialList = textInfo.ToTitleCase(model.MaterialList.Trim().ToLower()),
            };

            await _hatService.CreateHatsAsync(hat);
            TempData["Success"] = "Ny hatt skapades!";
            return RedirectToAction(nameof(Index));
        }

    }


}
