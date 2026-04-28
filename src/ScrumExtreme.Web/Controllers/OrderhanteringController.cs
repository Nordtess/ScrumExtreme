using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Web.Models;

namespace ScrumExtreme.Web.Controllers;

public class OrderhanteringController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IUserService _userService;
    private readonly IHatService _hatService;

    public OrderhanteringController(IOrderService orderService, IUserService userService, IHatService hatService)
    {
        _orderService = orderService;
        _userService = userService;
        _hatService = hatService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var orders = (await _orderService.GetAllOrdersAsync())
            .OrderByDescending(o => o.OrderDate)
            .ToList();

        var users = (await _userService.GetAllUsersAsync()).ToDictionary(u => u.Id);
        var hats = await _hatService.GetAllHatsAsync();

        static string StatusLabel(OrderStatus s) => s switch
        {
            OrderStatus.Pending => "Väntar",
            OrderStatus.Processing => "Påbörjad",
            OrderStatus.Printed => "Utskriven",
            OrderStatus.Shipped => "Skickad",
            _ => s.ToString()
        };

        static string StatusColor(OrderStatus s) => s switch
        {
            OrderStatus.Pending => "#b87c2a",
            OrderStatus.Processing => "#2e86ab",
            OrderStatus.Printed => "#c9a84c",
            OrderStatus.Shipped => "#2a7a4b",
            _ => "#888"
        };

        var vm = new OrderhanteringViewModel
        {
            TotalOrders = orders.Count,
            PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending),
            ProcessingOrders = orders.Count(o => o.Status == OrderStatus.Processing),
            PrintedOrders = orders.Count(o => o.Status == OrderStatus.Printed),
            ShippedOrders = orders.Count(o => o.Status == OrderStatus.Shipped),
            TotalCustomers = users.Values.Count(u => u.Role == "customer"),
            TotalHatModels = hats.Count(),
            RecentOrders = orders.Take(5).Select(o =>
            {
                users.TryGetValue(o.UserId, out var customer);
                return new OrderhanteringViewModel.RecentOrderRow
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = customer != null ? $"{customer.FirstName} {customer.LastName}" : "Okänd kund",
                    StatusLabel = StatusLabel(o.Status),
                    StatusColor = StatusColor(o.Status),
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount
                };
            }).ToList()
        };

        return View(vm);
    }
}
