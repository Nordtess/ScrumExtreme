using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Web.Models;

namespace ScrumExtreme.Web.Controllers;

[Route("Warehouse")]
public class WarehouseController : Controller
{
    private readonly IHatService _hatService;
    private readonly IItemService _itemService;

    public WarehouseController(IHatService hatService, IItemService itemService)
    {
        _hatService = hatService;
        _itemService = itemService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var hats = await _hatService.GetAllHatsAsync();
        var items = await _itemService.GetAllItemsAsync();
        return View(new WarehouseViewModel
        {
            Hats = hats,
            Items = items.OrderBy(i => i.Name)
        });
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

    [HttpPost("UpdateAllStock")]
    public async Task<IActionResult> UpdateAllStock([FromBody] UpdateAllStockRequest req)
    {
        if (string.IsNullOrEmpty(req.HatId) || req.Stock == null || req.Stock.Values.Any(v => v < 0))
            return BadRequest(new { error = "Ogiltiga parametrar." });

        var hat = await _hatService.GetByIdAsync(req.HatId);
        if (hat == null)
            return NotFound(new { error = "Hatten hittades inte." });

        foreach (var kv in req.Stock)
            hat.Stock[kv.Key] = kv.Value;

        await _hatService.UpdateHatAsync(hat);
        return Ok(new { success = true });
    }

    [HttpPost("CreateItem")]
    public async Task<IActionResult> CreateItem([FromBody] CreateItemRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name) || req.Price <= 0 || req.Stock < 0)
            return BadRequest(new { error = "Pris måste vara större än 0." });

        var existing = await _itemService.GetAllItemsAsync();
        if (existing.Any(i => i.Name.Equals(req.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            return Conflict(new { error = $"\"{req.Name}\" finns redan." });

        var textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
        var item = new Item
        {
            Name = textInfo.ToTitleCase(req.Name.Trim().ToLower()),
            Price = req.Price,
            Stock = req.Stock
        };

        await _itemService.CreateItemAsync(item);
        return Ok(new { success = true, id = item.Id, name = item.Name });
    }

    [HttpPost("UpdateItem")]
    public async Task<IActionResult> UpdateItem([FromBody] UpdateItemRequest req)
    {
        if (string.IsNullOrEmpty(req.ItemId) || req.Price < 0 || req.Stock < 0)
            return BadRequest(new { error = "Ogiltiga parametrar." });

        var item = await _itemService.GetByIdAsync(req.ItemId);
        if (item == null)
            return NotFound(new { error = "Tillbehöret hittades inte." });

        item.Price = req.Price;
        item.Stock = req.Stock;
        await _itemService.UpdateItemAsync(item);
        return Ok(new { success = true });
    }

    [HttpDelete("DeleteItem/{id}")]
    public async Task<IActionResult> DeleteItem(string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest(new { error = "Ogiltigt id." });

        var item = await _itemService.GetByIdAsync(id);
        if (item == null)
            return NotFound(new { error = "Tillbehöret hittades inte." });

        await _itemService.DeleteItemAsync(id);
        return Ok(new { success = true });
    }
}

public class UpdateStockRequest
{
    public string HatId { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class UpdateAllStockRequest
{
    public string HatId { get; set; } = string.Empty;
    public Dictionary<string, int> Stock { get; set; } = new();
}

public class CreateItemRequest
{
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public int Stock { get; set; }
}

public class UpdateItemRequest
{
    public string ItemId { get; set; } = string.Empty;
    public double Price { get; set; }
    public int Stock { get; set; }
}
