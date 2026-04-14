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
    private readonly IProductService _productService;

    public OrdersController(
        IOrderService orderService,
        IUserService userService,
        IProductService productService)
    {
        _orderService = orderService;
        _userService = userService;
        _productService = productService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        ViewBag.ProjectName = "ScrumExtreme";
        return View(orders);
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        var users = await _userService.GetAllUsersAsync();
        ViewBag.Customers = users.Where(u => !u.IsAdmin);

        var products = await _productService.GetAllProductsAsync();
        ViewBag.Products = products;

        ViewBag.ProjectName = "ScrumExtreme";
        return View();
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateOrderViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var users = await _userService.GetAllUsersAsync();
            ViewBag.Customers = users.Where(u => !u.IsAdmin);

            var products = await _productService.GetAllProductsAsync();
            ViewBag.Products = products;

            ViewBag.ProjectName = "ScrumExtreme";
            return View(model);
        }

        var product = await _productService.GetByIdAsync(model.ProductId);
        if (product == null)
        {
            ModelState.AddModelError(nameof(model.ProductId), "Produkten kunde inte hittas.");

            var users = await _userService.GetAllUsersAsync();
            ViewBag.Customers = users.Where(u => !u.IsAdmin);

            var products = await _productService.GetAllProductsAsync();
            ViewBag.Products = products;

            ViewBag.ProjectName = "ScrumExtreme";
            return View(model);
        }

        var order = new Order
        {
            UserId = model.CustomerId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Items = new List<OrderItem>
    {
        new OrderItem
        {
            ProductId = product.Id,
            Name = product.Name,
            Quantity = 1,
            UnitPrice = product.Price
        }
    }
};

        await _orderService.CreateOrderAsync(order);

        TempData["Success"] = "Förfrågan registrerad!";
        return RedirectToAction("Index");
    }
}