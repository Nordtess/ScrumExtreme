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
    private readonly IHatService _hatService;

    public OrdersController(
        IOrderService orderService,
        IUserService userService,
        IHatService hatService)
    {
        _orderService = orderService;
        _userService = userService;
        _hatService = hatService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        var users = await _userService.GetAllUsersAsync();
        ViewBag.EmailLookup = users.ToDictionary(u => u.Id, u => u.Email);
        return View(orders);
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        var users = await _userService.GetAllUsersAsync();
        ViewBag.Customers = users.Where(u => !u.IsAdmin);

        var hats = await _hatService.GetAllHatsAsync();
        ViewBag.Hats = hats;

        return View();
    }

    [HttpGet("CustomerInfo/{id}")]
    public async Task<IActionResult> CustomerInfo(string id)
    {
        var users = await _userService.GetAllUsersAsync();
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound();

        return Json(new
        {
            fullName = $"{user.FirstName} {user.LastName}",
            address = user.Address,
            city = user.City,
            postalCode = user.PostalCode,
            country = user.Country,
            countryCode = user.CountryCode,
            phone = user.PhoneNumber,
            email = user.Email
        });
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateOrderViewModel model)
    {
        if (string.IsNullOrEmpty(model.CustomerId) || model.Items == null || !model.Items.Any())
            return BadRequest(new { error = "Välj en kund och lägg till minst en hatt." });

        var users = await _userService.GetAllUsersAsync();
        var customer = users.FirstOrDefault(u => u.Id == model.CustomerId);
        if (customer == null)
            return BadRequest(new { error = "Kunden hittades inte." });

        var orderNumber = $"ORD-{DateTime.UtcNow.Year}-{Random.Shared.Next(1000, 9999)}";

        var order = new Order
        {
            OrderNumber = orderNumber,
            UserId = customer.Id,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            ShippingAddress = new ShippingAddress
            {
                FullName = $"{customer.FirstName} {customer.LastName}",
                Address = customer.Address,
                City = customer.City,
                PostalCode = customer.PostalCode,
                Country = customer.Country,
                CountryCode = customer.CountryCode,
                Phone = customer.PhoneNumber
            },
            Items = model.Items.Select(i => new OrderItem
            {
                ProductId = i.HatId,
                Name = i.Name,
                Quantity = 1,
                UnitPrice = i.UnitPrice
            }).ToList(),
            TotalAmount = model.Items.Sum(i => i.UnitPrice)
        };

        await _orderService.CreateOrderAsync(order);
        return Ok(new { success = true });
    }
}
