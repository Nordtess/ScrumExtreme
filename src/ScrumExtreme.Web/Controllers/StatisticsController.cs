using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Web.Models;

public class StatisticsController : Controller
{
    private readonly IStatisticsService _statisticsService;
    private readonly IOrderService _orderService;
    private readonly IUserService _userService;

    public StatisticsController(IStatisticsService statisticsService, IOrderService orderService, IUserService userService)
    {
        _statisticsService = statisticsService;
        _orderService = orderService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string period = "month")
    {
        if (period != "week" && period != "month" && period != "quarter" && period != "year")
            period = "month";

        var result = await _statisticsService.GetStatisticsAsync(period);
        return View(result);
    }

    [HttpGet("Statistics/CustomerStats")]
    public async Task<IActionResult> CustomerStats()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        var users = await _userService.GetAllUsersAsync();

        var statusLabel = (OrderStatus s) => s switch
        {
            OrderStatus.Pending => "Väntar",
            OrderStatus.Processing => "Påbörjad",
            OrderStatus.Printed => "Utskriven",
            OrderStatus.Shipped => "Skickad",
            _ => s.ToString()
        };

        var customerOrders = orders
            .GroupBy(o => o.UserId)
            .Select(g =>
            {
                var user = users.FirstOrDefault(u => u.Id == g.Key);
                var topHat = g.SelectMany(o => o.Items)
                              .GroupBy(i => i.Name)
                              .OrderByDescending(x => x.Sum(i => i.Quantity))
                              .Select(x => x.Key)
                              .FirstOrDefault() ?? "–";
                return new CustomerStatRow
                {
                    CustomerId = g.Key,
                    FullName = user != null ? $"{user.FirstName} {user.LastName}" : "Okänd kund",
                    Email = user?.Email ?? "–",
                    OrderCount = g.Count(),
                    TotalSpent = g.Sum(o => o.TotalAmount),
                    TopHat = topHat,
                    Orders = g.OrderByDescending(o => o.OrderDate).Select(o => new OrderHistoryRow
                    {
                        OrderNumber = o.OrderNumber,
                        OrderDate = o.OrderDate,
                        Amount = o.TotalAmount,
                        Status = statusLabel(o.Status),
                        OrderId = o.Id
                    }).ToList()
                };
            })
            .OrderByDescending(r => r.TotalSpent)
            .ToList();

        return View(customerOrders);
    }

    [HttpGet("Statistics/HatStats")]
    public async Task<IActionResult> HatStats()
    {
        var orders = await _orderService.GetAllOrdersAsync();

        var hatStats = orders
            .SelectMany(o => o.Items)
            .GroupBy(i => i.Name)
            .Select(g => new HatStatRow
            {
                HatName = g.Key,
                TotalOrders = g.Select(i => i).Count(),
                TotalUnits = g.Sum(i => i.Quantity),
                TotalRevenue = g.Sum(i => i.Quantity * i.UnitPrice)
            })
            .OrderByDescending(r => r.TotalUnits)
            .ToList();

        return View(hatStats);
    }
}
