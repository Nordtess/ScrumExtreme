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
    private readonly IMaterialService _materialService;

    public WarehouseController(IHatService hatService, IItemService itemService, IMaterialService materialService)
    {
        _hatService = hatService;
        _itemService = itemService;
        _materialService = materialService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var hats = await _hatService.GetAllHatsAsync();
        var items = await _itemService.GetAllItemsAsync();
        var materials = await _materialService.GetMaterialsAsync();
        return View(new WarehouseViewModel
        {
            Hats = hats,
            Items = items.OrderBy(i => i.Name),
            Materials = materials.OrderBy(m => m.Name)
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

    [HttpPost("SaveHatStock")]
    public async Task<IActionResult> SaveHatStock([FromBody] SaveHatStockRequest req)
    {
        if (string.IsNullOrEmpty(req.HatId) || req.Stock == null || req.Stock.Values.Any(v => v < 0))
            return BadRequest(new { error = "Ogiltiga parametrar." });

        var hat = await _hatService.GetByIdAsync(req.HatId);
        if (hat == null)
            return NotFound(new { error = "Hatten hittades inte." });

        int oldTotal = hat.Stock.Values.Sum();
        int newTotal = req.Stock.Values.Sum();
        int delta = newTotal - oldTotal;

        var updatedMaterialStocks = new Dictionary<string, int>();

        if (delta > 0 && !string.IsNullOrWhiteSpace(hat.MaterialList))
        {
            var materialNames = hat.MaterialList
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(n => n.Trim())
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList();

            if (materialNames.Count > 0)
            {
                var allMaterials = (await _materialService.GetMaterialsAsync()).ToList();
                var shortages = new List<string>();

                foreach (var name in materialNames)
                {
                    var mat = allMaterials.FirstOrDefault(m =>
                        m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    if (mat == null) continue;
                    if (mat.Stock < delta)
                        shortages.Add($"{mat.Name} (behövs {delta}, finns {mat.Stock})");
                }

                if (shortages.Count > 0)
                    return Conflict(new { error = "Inte tillräckligt material: " + string.Join(", ", shortages) });

                foreach (var name in materialNames)
                {
                    var mat = allMaterials.FirstOrDefault(m =>
                        m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    if (mat == null) continue;
                    mat.Stock -= delta;
                    await _materialService.UpdateMaterialAsync(mat);
                    updatedMaterialStocks[mat.Id] = mat.Stock;
                }
            }
        }

        foreach (var kv in req.Stock)
            hat.Stock[kv.Key] = kv.Value;
        await _hatService.UpdateHatAsync(hat);

        return Ok(new { success = true, materialStocks = updatedMaterialStocks });
    }

    [HttpDelete("DeleteHat/{id}")]
    public async Task<IActionResult> DeleteHat(string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest(new { error = "Ogiltigt id." });

        var hat = await _hatService.GetByIdAsync(id);
        if (hat == null)
            return NotFound(new { error = "Hatten hittades inte." });

        await _hatService.DeleteHatAsync(id);
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

    [HttpPost("CreateMaterial")]
    public async Task<IActionResult> CreateMaterial([FromBody] CreateMaterialRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name) || req.Price <= 0 || req.Stock < 0)
            return BadRequest(new { error = "Pris måste vara större än 0." });

        var existing = await _materialService.GetMaterialsAsync();
        if (existing.Any(m => m.Name.Equals(req.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            return Conflict(new { error = $"\"{req.Name}\" finns redan." });

        var textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
        var material = new Material
        {
            Name = textInfo.ToTitleCase(req.Name.Trim().ToLower()),
            Price = req.Price,
            Stock = req.Stock
        };

        await _materialService.CreateMaterialAsync(material);
        return Ok(new { success = true, id = material.Id, name = material.Name });
    }

    [HttpPost("UpdateMaterial")]
    public async Task<IActionResult> UpdateMaterial([FromBody] UpdateMaterialRequest req)
    {
        if (string.IsNullOrEmpty(req.MaterialId) || req.Price < 0 || req.Stock < 0)
            return BadRequest(new { error = "Ogiltiga parametrar." });

        var material = await _materialService.GetMaterialByIdAsync(req.MaterialId);
        if (material == null)
            return NotFound(new { error = "Materialet hittades inte." });

        material.Price = req.Price;
        material.Stock = req.Stock;
        await _materialService.UpdateMaterialAsync(material);
        return Ok(new { success = true });
    }

    [HttpDelete("DeleteMaterial/{id}")]
    public async Task<IActionResult> DeleteMaterial(string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest(new { error = "Ogiltigt id." });

        var material = await _materialService.GetMaterialByIdAsync(id);
        if (material == null)
            return NotFound(new { error = "Materialet hittades inte." });

        await _materialService.DeleteMaterialAsync(id);
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

public class SaveHatStockRequest
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

public class CreateMaterialRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

public class UpdateMaterialRequest
{
    public string MaterialId { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
