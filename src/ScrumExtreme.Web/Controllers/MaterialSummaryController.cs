using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Web.Models;

namespace ScrumExtreme.Web.Controllers
{
    public class MaterialSummaryController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IHatService _hatService;
        private readonly IMaterialService _materialService;
        private readonly IItemService _itemService;
        private readonly ICompanySettingsService _companySettingsService;

        public MaterialSummaryController(
            IOrderService orderService,
            IHatService hatService,
            IMaterialService materialService,
            IItemService itemService,
            ICompanySettingsService companySettingsService)
        {
            _orderService = orderService;
            _hatService = hatService;
            _materialService = materialService;
            _itemService = itemService;
            _companySettingsService = companySettingsService;
        }

        private async Task<MaterialSummaryPageViewModel> BuildSummaryAsync()
        {
            var orders = (await _orderService.GetPendingOrdersAsync()).ToList();
            var hats = await _hatService.GetAllHatsAsync();
            var materials = await _materialService.GetMaterialsAsync();
            var items = await _itemService.GetAllItemsAsync();

            var hatLookup = hats.ToDictionary(h => h.Name, h => h.MaterialList);
            var priceLookup = materials.ToDictionary(m => m.Name, m => m.Price, StringComparer.OrdinalIgnoreCase);
            var itemLookup = items.ToDictionary(i => i.Id, i => i);

            var materialSummary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var itemSummary = new Dictionary<string, (int qty, decimal price)>(StringComparer.OrdinalIgnoreCase);

            foreach (var order in orders)
            {
                foreach (var orderItem in order.Items)
                {
                    // Materials from hat's MaterialList
                    if (hatLookup.TryGetValue(orderItem.Name, out var materialList))
                    {
                        foreach (var name in materialList
                            .Split(',')
                            .Select(m => m.Trim())
                            .Where(m => !string.IsNullOrEmpty(m)))
                        {
                            if (!materialSummary.ContainsKey(name))
                                materialSummary[name] = 0;
                            materialSummary[name] += orderItem.Quantity;
                        }
                    }

                    // Items/accessories from OrderItem.ItemIds
                    foreach (var itemId in orderItem.ItemIds)
                    {
                        if (!itemLookup.TryGetValue(itemId, out var item)) continue;

                        if (!itemSummary.ContainsKey(item.Name))
                            itemSummary[item.Name] = (0, (decimal)item.Price);

                        var (qty, price) = itemSummary[item.Name];
                        itemSummary[item.Name] = (qty + orderItem.Quantity, price);
                    }
                }
            }

            var capitalSEK = await _companySettingsService.GetCapitalAsync();

            return new MaterialSummaryPageViewModel
            {
                Materials = materialSummary.Select(x => new MaterialSummaryViewModel
                {
                    MaterialName = x.Key,
                    TotalQuantity = x.Value,
                    PricePerUnit = priceLookup.TryGetValue(x.Key, out var price) ? price : 0m
                }).ToList(),

                Items = itemSummary.Select(x => new ItemSummaryViewModel
                {
                    ItemName = x.Key,
                    TotalQuantity = x.Value.qty,
                    PricePerUnit = x.Value.price
                }).ToList(),

                CapitalSEK = capitalSEK
            };
        }

        public async Task<IActionResult> PrintMaterialSummary()
        {
            var model = await BuildSummaryAsync();
            return View(model);
        }

        public async Task<IActionResult> PrintOrder()
        {
            var model = await BuildSummaryAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder()
        {
            var orders = (await _orderService.GetPendingOrdersAsync()).ToList();
            var hats = await _hatService.GetAllHatsAsync();
            var allMaterials = (await _materialService.GetMaterialsAsync()).ToList();
            var allItems = (await _itemService.GetAllItemsAsync()).ToList();

            var hatLookup = hats.ToDictionary(h => h.Name, h => h.MaterialList);
            var materialMap = allMaterials.ToDictionary(m => m.Name, m => m, StringComparer.OrdinalIgnoreCase);
            var itemLookup = allItems.ToDictionary(i => i.Id, i => i);

            var materialQty = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var itemQty = new Dictionary<string, int>();

            foreach (var order in orders)
            {
                foreach (var orderItem in order.Items)
                {
                    if (hatLookup.TryGetValue(orderItem.Name, out var materialList))
                    {
                        foreach (var name in materialList
                            .Split(',')
                            .Select(m => m.Trim())
                            .Where(m => !string.IsNullOrEmpty(m)))
                        {
                            if (!materialQty.ContainsKey(name)) materialQty[name] = 0;
                            materialQty[name] += orderItem.Quantity;
                        }
                    }

                    foreach (var itemId in orderItem.ItemIds)
                    {
                        if (!itemLookup.ContainsKey(itemId)) continue;
                        if (!itemQty.ContainsKey(itemId)) itemQty[itemId] = 0;
                        itemQty[itemId] += orderItem.Quantity;
                    }
                }

                order.Status = OrderStatus.Printed;
                await _orderService.UpdateAsync(order);
            }

            // Restock materials
            decimal totalCost = 0m;
            foreach (var (name, qty) in materialQty)
            {
                if (materialMap.TryGetValue(name, out var mat))
                {
                    totalCost += mat.Price * qty;
                    mat.Stock += qty;
                    await _materialService.UpdateMaterialAsync(mat);
                }
            }

            // Restock items
            foreach (var (id, qty) in itemQty)
            {
                if (itemLookup.TryGetValue(id, out var item))
                {
                    totalCost += (decimal)item.Price * qty;
                    item.Stock += qty;
                    await _itemService.UpdateItemAsync(item);
                }
            }

            // Deduct material order cost from company capital
            await _companySettingsService.DeductCapitalAsync(totalCost);

            TempData["Success"] = "Materialbeställning bekräftad. Lagersaldo har uppdaterats.";
            return RedirectToAction("PrintMaterialSummary");
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPrinted()
        {
            var orders = await _orderService.GetPendingOrdersAsync();

            foreach (var order in orders)
            {
                order.Status = OrderStatus.Printed;
                await _orderService.UpdateAsync(order);
            }

            TempData["Success"] = "Orders marked as printed.";
            return RedirectToAction("PrintMaterialSummary");
        }
    }
}
