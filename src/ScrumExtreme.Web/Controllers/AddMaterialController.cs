using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Web.Models;

public class AddMaterialController : Controller
{
    private readonly IMaterialService _materialService;

    public AddMaterialController(IMaterialService materialService)
    {
        _materialService = materialService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new CreateMaterialViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Index(CreateMaterialViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Index", model);

        var material = new Material
        {
            Name = model.Name,
            Price = model.Price
        };

        await _materialService.CreateMaterialAsync(material);

        TempData["Success"] = "Material tillagt!";
        return RedirectToAction(nameof(Index));
    }
}