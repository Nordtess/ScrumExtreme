using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;

namespace ScrumExtreme.Web.Controllers;

[Route("Statistics")]
public class SalesStatisticsController : Controller
{
    private readonly IOrderService _orderService;

    public SalesStatisticsController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetAllOrdersAsync();

        var totalSales = orders.Sum(o => o.TotalAmount);
        var totalOrders = orders.Count();

        ViewBag.TotalSales = totalSales;
        ViewBag.TotalOrders = totalOrders;
        ViewBag.Orders = orders;

        return View();
    }
}

