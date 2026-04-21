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

        public MaterialSummaryController(
            IOrderService orderService,
            IHatService hatService)
        {
            _orderService = orderService;
            _hatService = hatService;
        }

        public async Task<IActionResult> PrintMaterialSummary()
        {
            var orders = await _orderService.GetPendingOrdersAsync();
            var hats = await _hatService.GetAllHatsAsync();

            var hatLookup = hats.ToDictionary(h => h.Name, h => h.MaterialList);

            var summary = new Dictionary<string, int>();

            foreach (var order in orders)
            {
                foreach (var item in order.Items)
                {
                    if (!hatLookup.ContainsKey(item.Name))
                        continue;

                    var materials = hatLookup[item.Name]
                        .Split(',')
                        .Select(m => m.Trim());

                    foreach (var material in materials)
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
                TotalQuantity = x.Value
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