using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;

namespace ScrumExtreme.Web.Controllers;

[Route("Warehouse")]
public class WarehouseController : Controller
{
    private readonly IHatService _hatService;

    public WarehouseController(IHatService hatService)
    {
        _hatService = hatService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var hats = await _hatService.GetAllHatsAsync();
        return View(hats);
    }

    [HttpPost("UpdateStock")]
    public async Task<IActionResult> UpdateStock([FromBody] UpdateStockRequest req)
    {
        if (string.IsNullOrEmpty(req.HatId) || string.IsNullOrEmpty(req.Size) || req.Quantity < 0)
            return BadRequest(new { error = "Ogiltiga parametrar." });

        var hat = await _hatService.GetByIdAsync(req.HatId);
        if (hat == null)
            return NotFound(new { error = "Hatten hittades inte." });

        hat.Stock[req.Size] = req.Quantity;
        await _hatService.UpdateHatAsync(hat);
        return Ok(new { success = true });
    }
}

public class UpdateStockRequest
{
    public string HatId { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
