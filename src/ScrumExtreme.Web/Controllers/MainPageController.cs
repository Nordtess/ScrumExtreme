using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;

namespace ScrumExtreme.Web.Controllers;

[Route("MainPage")]
public class MainPageController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IUserService _userService;

    public MainPageController(IOrderService orderService, IUserService userService)
    {
        _orderService = orderService;
        _userService = userService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var allOrders = await _orderService.GetAllOrdersAsync();
        var orders = allOrders.Where(o => o.OrderDate.Year == DateTime.UtcNow.Year);
        var users = await _userService.GetAllUsersAsync();
        ViewBag.UserLookup = users.ToDictionary(u => u.Id, u => u);
        return View(orders);
    }
}
