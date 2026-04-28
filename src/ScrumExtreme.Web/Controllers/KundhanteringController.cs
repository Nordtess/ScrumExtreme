using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Web.Models;

namespace ScrumExtreme.Web.Controllers;

public class KundhanteringController : Controller
{
    private readonly IUserService _userService;
    private readonly IOrderService _orderService;

    public KundhanteringController(IUserService userService, IOrderService orderService)
    {
        _userService = userService;
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var allUsers = (await _userService.GetAllUsersAsync()).ToList();
        var customers = allUsers.Where(u => u.Role == "customer").ToList();
        var orders = await _orderService.GetAllOrdersAsync();

        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var vm = new KundhanteringViewModel
        {
            TotalCustomers = customers.Count,
            CustomersThisMonth = customers.Count(u => u.CreatedAt >= startOfMonth),
            TotalOrders = orders.Count(),
            RecentCustomers = customers
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .Select(u => new KundhanteringViewModel.RecentCustomerRow
                {
                    FullName = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    City = u.City,
                    Country = u.Country
                })
                .ToList()
        };

        return View(vm);
    }
}
