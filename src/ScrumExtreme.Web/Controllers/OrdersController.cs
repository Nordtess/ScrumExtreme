using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public async Task<IActionResult> Index()
    {
        await LoadUsersAsync();

        var orders = await _orderService.GetAllOrdersAsync();
        ViewBag.Orders = orders.OrderByDescending(o => o.OrderDate).ToList();

        return View(new CreateOrderViewModel());
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CreateOrderViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadUsersAsync();
            ViewBag.Orders = (await _orderService.GetAllOrdersAsync())
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(model);
        }

        var user = await _userService.GetByIdAsync(model.UserId);
        if (user == null)
        {
            ModelState.AddModelError(nameof(model.UserId), "Kunden kunde inte hittas.");

            await LoadUsersAsync();
            ViewBag.Orders = (await _orderService.GetAllOrdersAsync())
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(model);
        }

        var order = new Order
        {
            UserId = user.Id,
            ShippingAddress = new ShippingAddress
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                City = user.City,
                PostalCode = user.PostalCode,
                CountryCode = user.CountryCode,
                Country = user.Country,
                PhoneNumber = user.PhoneNumber
            },
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = model.ProductId,
                    Name = model.ProductName,
                    Quantity = model.Quantity,
                    UnitPrice = model.UnitPrice
                }
            }
        };

        await _orderService.CreateOrderAsync(order);

        TempData["SuccessMessage"] = "Ordern sparades.";

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadUsersAsync()
    {
        var users = await _userService.GetAllUsersAsync();

        ViewBag.Users = users
            .Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = $"{u.FirstName} {u.LastName} ({u.Email})"
            })
            .ToList();
    }
}