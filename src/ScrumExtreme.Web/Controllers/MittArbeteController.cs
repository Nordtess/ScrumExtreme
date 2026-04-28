using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Web.Models;

namespace ScrumExtreme.Web.Controllers;

public class MittArbeteController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IUserService _userService;
    private readonly ICalendarEventService _calendarEventService;

    public MittArbeteController(
        IOrderService orderService,
        IUserService userService,
        ICalendarEventService calendarEventService)
    {
        _orderService = orderService;
        _userService = userService;
        _calendarEventService = calendarEventService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var sessionUserId = HttpContext.Session.GetString("UserId") ?? "";

        var allOrders = await _orderService.GetAllOrdersAsync();
        var allUsers = (await _userService.GetAllUsersAsync()).ToDictionary(u => u.Id);
        var myEvents = (await _calendarEventService.GetByUserIdAsync(sessionUserId)).ToList();

        var myOrders = allOrders
            .Where(o => o.AssignedWorkerId == sessionUserId)
            .OrderByDescending(o => o.OrderDate)
            .ToList();

        var activeOrders = myOrders.Where(o => o.Status != OrderStatus.Shipped).ToList();
        var completedOrders = myOrders.Count(o => o.Status == OrderStatus.Shipped);

        var now = DateTime.UtcNow;
        var upcomingEvents = myEvents
            .Where(e => e.End >= now)
            .OrderBy(e => e.Start)
            .ToList();

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

        static string EventTypeLabel(string? t, string? orderId) =>
            !string.IsNullOrEmpty(orderId) ? "Order" :
            t switch
            {
                "Ledighet" => "Ledighet",
                "Sjukfrånvaro" => "Sjukfrånvaro",
                _ => t ?? "Övrigt"
            };

        static string EventTypeColor(string? t, string? orderId) =>
            !string.IsNullOrEmpty(orderId) ? "#2e86ab" :
            t switch
            {
                "Ledighet" => "#3b7d4f",
                "Sjukfrånvaro" => "#c73e1d",
                _ => "#888"
            };

        allUsers.TryGetValue(sessionUserId, out var currentUser);

        var vm = new MittArbeteViewModel
        {
            WorkerName = currentUser != null
                ? $"{currentUser.FirstName} {currentUser.LastName}"
                : HttpContext.Session.GetString("UserFullName") ?? "",
            ActiveOrders = activeOrders.Count,
            CompletedOrders = completedOrders,
            UpcomingEvents = upcomingEvents.Count,
            MyOrders = activeOrders.Select(o =>
            {
                allUsers.TryGetValue(o.UserId, out var customer);
                return new MittArbeteViewModel.MyOrderRow
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = customer != null
                        ? $"{customer.FirstName} {customer.LastName}"
                        : "Okänd kund",
                    StatusLabel = StatusLabel(o.Status),
                    StatusColor = StatusColor(o.Status),
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount
                };
            }).ToList(),
            MyEvents = upcomingEvents.Select(e => new MittArbeteViewModel.MyEventRow
            {
                TypeLabel = EventTypeLabel(e.EventType, e.OrderId),
                TypeColor = EventTypeColor(e.EventType, e.OrderId),
                Start = e.Start,
                End = e.End
            }).ToList()
        };

        return View(vm);
    }
}
