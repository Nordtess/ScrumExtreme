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

        public MaterialSummaryController(
            IOrderService orderService,
            IHatService hatService,
            IMaterialService materialService)
        {
            _orderService = orderService;
            _hatService = hatService;
            _materialService = materialService;
        }

        public async Task<IActionResult> PrintMaterialSummary()
        {
            var orders = await _orderService.GetPendingOrdersAsync();
            var hats = await _hatService.GetAllHatsAsync();
            var materials = await _materialService.GetMaterialsAsync();

            var hatLookup = hats.ToDictionary(h => h.Name, h => h.MaterialList);
            var priceLookup = materials.ToDictionary(m => m.Name, m => m.Price, StringComparer.OrdinalIgnoreCase);

            var summary = new Dictionary<string, int>();

            foreach (var order in orders)
            {
                foreach (var item in order.Items)
                {
                    if (!hatLookup.ContainsKey(item.Name))
                        continue;

                    var materialNames = hatLookup[item.Name]
                        .Split(',')
                        .Select(m => m.Trim());

                    foreach (var material in materialNames)
                    {
                        if (!summary.ContainsKey(material))
                            summary[material] = 0;

                        summary[material] += item.Quantity;
                    }
                }
            }

            var model = summary.Select(x => new MaterialSummaryViewModel
            {
                MaterialName = x.Key,
                TotalQuantity = x.Value,
                PricePerUnit = priceLookup.TryGetValue(x.Key, out var price) ? price : 0m
            }).ToList();

            return View(model);
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