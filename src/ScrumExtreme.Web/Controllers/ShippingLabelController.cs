using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Web.Models;

namespace ScrumExtreme.Web.Controllers;

[Route("ShippingLabel")]
public class ShippingLabelController : Controller
{
    private readonly IOrderService _orderService;

    public ShippingLabelController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? orderId)
    {
        if (string.IsNullOrEmpty(orderId))
            return View(new ShippingLabelViewModel());

        var order = await _orderService.GetByIdAsync(orderId);
        if (order == null)
            return View(new ShippingLabelViewModel());

        var addr = order.ShippingAddress;
        var model = new ShippingLabelViewModel
        {
            HasOrder = true,
            FullName = addr.FullName,
            Address = addr.Address,
            PostalCode = addr.PostalCode,
            City = addr.City,
            Country = addr.Country,
            CountryCode = addr.CountryCode,
            Phone = addr.Phone,
            OrderNumber = order.OrderNumber
        };

        return View(model);
    }
}
