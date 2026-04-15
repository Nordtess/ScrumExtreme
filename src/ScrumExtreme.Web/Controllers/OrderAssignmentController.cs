using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Web.Controllers;

[Route("Orders/Assignment")]
public class OrderAssignmentController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IUserService _userService;

    public OrderAssignmentController(IOrderService orderService, IUserService userService)
    {
        _orderService = orderService;
        _userService = userService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        TempData["Error"] = null;
        TempData["Success"] = null;

        var orders = await _orderService.GetAllOrdersAsync();
        var users = await _userService.GetAllUsersAsync();

        ViewBag.Orders = orders;
        ViewBag.Customers = users
            .Where(u => !u.IsAdmin && !u.IsEmployee)
            .ToList();

        return View("~/Views/AssignCustomer/Index.cshtml");
    }

    [HttpPost("Assign")]
    public async Task<IActionResult> Assign(string selectedOrderId, string userId)
    {
        if (string.IsNullOrEmpty(selectedOrderId) || string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "Du måste välja både order och kund.";
            return RedirectToAction("Index");
        }

        var order = await _orderService.GetByIdAsync(selectedOrderId);

        if (order == null)
            return NotFound();

        order.UserId = userId;

        await _orderService.UpdateOrderAsync(order);

        TempData["Success"] = "Kund tilldelad till order!";
        return RedirectToAction("Index");
    }
}