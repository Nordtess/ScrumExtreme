using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Web.Models;

namespace ScrumExtreme.Web.Controllers;

public class LagerhanteringController : Controller
{
    private readonly IHatService _hatService;
    private readonly IItemService _itemService;
    private readonly IMaterialService _materialService;
    private readonly ICompanySettingsService _companySettingsService;
    private readonly IOrderService _orderService;
    private readonly IStatisticsService _statisticsService;

    public LagerhanteringController(
        IHatService hatService,
        IItemService itemService,
        IMaterialService materialService,
        ICompanySettingsService companySettingsService,
        IOrderService orderService,
        IStatisticsService statisticsService)
    {
        _hatService = hatService;
        _itemService = itemService;
        _materialService = materialService;
        _companySettingsService = companySettingsService;
        _orderService = orderService;
        _statisticsService = statisticsService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var hats = (await _hatService.GetAllHatsAsync()).ToList();
        var items = (await _itemService.GetAllItemsAsync()).ToList();
        var materials = (await _materialService.GetMaterialsAsync()).ToList();
        var capital = await _companySettingsService.GetCapitalAsync();
        var pendingOrders = (await _orderService.GetPendingOrdersAsync()).ToList();
        var stats = await _statisticsService.GetStatisticsAsync("month");

        var totalStock = hats.Sum(h => h.Stock.Values.Sum());

        var hatLookup = hats.ToDictionary(h => h.Name, h => h.MaterialList);
        var materialsNeeded = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var order in pendingOrders)
        {
            foreach (var item in order.Items)
            {
                if (hatLookup.TryGetValue(item.Name, out var matList))
                {
                    foreach (var m in matList.Split(',').Select(x => x.Trim()).Where(x => x.Length > 0))
                        materialsNeeded.Add(m);
                }
            }
        }

        var vm = new LagerhanteringViewModel
        {
            TotalHatTypes = hats.Count,
            TotalHatsInStock = totalStock,
            TotalMaterials = materials.Count,
            TotalItems = items.Count,
            CapitalSEK = capital,
            PendingOrderCount = pendingOrders.Count,
            UniqueMaterialsNeeded = materialsNeeded.Count,
            TotalMaterialCost = pendingOrders
                .SelectMany(o => o.Items)
                .Sum(i => (decimal)i.UnitPrice * i.Quantity),
            TotalRevenueLast30Days = stats.TotalRevenue,
            TotalCostsLast30Days = stats.TotalCosts,
            OrdersLast30Days = stats.TotalOrders
        };

        return View(vm);
    }
}
