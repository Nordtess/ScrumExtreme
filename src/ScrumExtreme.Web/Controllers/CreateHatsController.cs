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
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var hat = new Hats
            {
                Name = model.Name,
                Size = model.Size,
                Price = model.Price,
                MaterialList = model.MaterialList,

            };

            await _hatService.CreateHatsAsync(hat);
            TempData["Success"] = "Ny hatt skapades!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("GetAllHats")]
        public async Task<IActionResult> GetAllHats()
        {
            var hats = await _hatService.GetAllHatsAsync();
            return View(hats);
        }
    }

}
