using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Web.Models;

namespace ScrumExtreme.Web.Controllers;

[Route("Orders")]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IUserService _userService;

    public OrdersController(IOrderService orderService, IUserService userService)
    {
        _orderService = orderService;
        _userService = userService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        var users = await _userService.GetAllUsersAsync();
        ViewBag.Customers = users.Where(u => !u.IsAdmin);

        return View();
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateOrderViewModel model)
    {
        if (!ModelState.IsValid)
        {
            
            var users = await _userService.GetAllUsersAsync();
            ViewBag.Customers = users.Where(u => !u.IsAdmin);

            return View(model);
        }

        var order = new Order
        {
            UserId = model.CustomerId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };

        await _orderService.CreateOrderAsync(order);

        TempData["Success"] = "Förfrågan registrerad!";
        return RedirectToAction("Index");
    }
}