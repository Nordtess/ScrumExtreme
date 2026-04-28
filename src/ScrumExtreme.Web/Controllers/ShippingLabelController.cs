using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Web.Models;

namespace ScrumExtreme.Web.Controllers;

[Route("ShippingLabel")]
public class ShippingLabelController : Controller
{
    private readonly IOrderService _orderService;
    private readonly ICalendarEventService _calendarEventService;

    public ShippingLabelController(IOrderService orderService, ICalendarEventService calendarEventService)
    {
        _orderService = orderService;
        _calendarEventService = calendarEventService;
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
            OrderId = order.Id,
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

    [HttpPost("MarkAsShipped")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsShipped(string orderId)
    {
        if (!string.IsNullOrEmpty(orderId))
        {
            var order = await _orderService.GetByIdAsync(orderId);
            if (order != null)
            {
                // Delete the calendar event for this order
                if (!string.IsNullOrEmpty(order.AssignedWorkerId))
                {
                    var workerEvents = await _calendarEventService.GetByUserIdAsync(order.AssignedWorkerId);
                    var orderEvent = workerEvents.FirstOrDefault(e => e.OrderId == orderId);
                    if (orderEvent != null)
                        await _calendarEventService.DeleteCalendarEventAsync(orderEvent.Id);
                }

                order.Status = OrderStatus.Shipped;
                order.AssignedWorkerId = null;
                await _orderService.UpdateAsync(order);
            }
        }
        return RedirectToAction("Index", "Orders");
    }
}
