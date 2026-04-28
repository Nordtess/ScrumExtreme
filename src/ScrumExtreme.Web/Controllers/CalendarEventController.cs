using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Domain.Interfaces;


namespace ScrumExtreme.Web.Controllers
{
    public class CalendarController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly ICalendarEventService _calendarEventService;


        public CalendarController(IOrderService orderService, IUserService userService, ICalendarEventService calendarEventService,
            IRepository<CalendarEvent> events,
            IRepository<User> users,
            IRepository<Order> orders)
        {
            _orderService = orderService;
            _userService = userService;
            _calendarEventService = calendarEventService;

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetEvents()
        {
            var events = await _calendarEventService.GetAllCalendarEventsAsync();
            var users = await _userService.GetAllUsersAsync();
            var orders = await _orderService.GetAllOrdersAsync();

            var userDict = users.ToDictionary(u => u.Id);
            var orderDict = orders.ToDictionary(o => o.Id);

            string[] palette = ["#c9a84c", "#2e86ab", "#a23b72", "#f18f01", "#c73e1d", "#3b7d4f"];
            var workerIds = events
                .Where(e => !string.IsNullOrEmpty(e.OrderId))
                .Select(e => e.UserId)
                .Distinct()
                .ToList();
            var workerColors = workerIds
                .Select((id, i) => (id, color: palette[i % palette.Length]))
                .ToDictionary(x => x.id, x => x.color);

            static string TypeColor(string? t) => (t ?? "").ToLower() switch
            {
                "ledighet" => "#3b7d4f",
                "sjukfr\u00e5nvaro" => "#c73e1d",
                _ => "#888"
            };

            var result = new List<object>();

            foreach (var e in events)
            {
                if (!userDict.ContainsKey(e.UserId)) continue;
                var user = userDict[e.UserId];
                var workerName = $"{user.FirstName} {user.LastName}";

                bool isOrderEvent = !string.IsNullOrEmpty(e.OrderId) && orderDict.ContainsKey(e.OrderId);

                if (isOrderEvent)
                {
                    result.Add(new
                    {
                        id = e.Id,
                        title = $"{user.FirstName} \u2014 {orderDict[e.OrderId!].OrderNumber}",
                        start = e.Start,
                        end = e.End,
                        color = workerColors.TryGetValue(e.UserId, out var c) ? c : palette[0],
                        extendedProps = new
                        {
                            orderId = e.OrderId,
                            orderNumber = orderDict[e.OrderId!].OrderNumber,
                            workerName
                        }
                    });
                }
                else
                {
                    var label = e.EventType ?? "event";
                    var displayLabel = char.ToUpper(label[0]) + label[1..];
                    result.Add(new
                    {
                        id = e.Id,
                        title = $"{user.FirstName} \u2014 {displayLabel}",
                        start = e.Start,
                        end = e.End,
                        color = TypeColor(e.EventType),
                        extendedProps = new
                        {
                            orderId = (string?)null,
                            orderNumber = (string?)null,
                            workerName
                        }
                    });
                }
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> CreateEvent([FromBody] CreateCalendarEventRequest req)
        {
            if (req == null)
                return Json(new { success = false, error = "Invalid request body" });

            // Optionally update the order's status
            if (req.EventType == "order" && !string.IsNullOrEmpty(req.OrderStatusOverride) && !string.IsNullOrEmpty(req.OrderId))
            {
                var order = await _orderService.GetByIdAsync(req.OrderId);
                if (order != null && Enum.TryParse<OrderStatus>(req.OrderStatusOverride, ignoreCase: true, out var newStatus))
                {
                    order.Status = newStatus;
                    await _orderService.UpdateAsync(order);
                }
            }

            var ev = new CalendarEvent
            {
                UserId = req.UserId,
                OrderId = req.EventType == "order" ? req.OrderId : null,
                EventType = req.EventType,
                Start = req.Start ?? DateTime.UtcNow.Date,
                End = req.End ?? DateTime.UtcNow.Date
            };
            await _calendarEventService.CreateEventAsync(ev);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<JsonResult> UpdateEvent([FromBody] CalendarEvent model)
        {
            await _calendarEventService.UpdateAsync(model);
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<JsonResult> GetEventsForEdit()
        {
            var events = await _calendarEventService.GetAllCalendarEventsAsync();
            var users = await _userService.GetAllUsersAsync();
            var orders = await _orderService.GetAllOrdersAsync();

            var userDict = users.ToDictionary(u => u.Id);
            var orderDict = orders.ToDictionary(o => o.Id);

            var result = events
                .Where(e => userDict.ContainsKey(e.UserId))
                .Select(e =>
                {
                    var user = userDict[e.UserId];
                    var isOrder = !string.IsNullOrEmpty(e.OrderId) && orderDict.ContainsKey(e.OrderId);
                    var label = isOrder ? orderDict[e.OrderId!].OrderNumber
                                         : (e.EventType is { Length: > 0 } t
                                            ? char.ToUpper(t[0]) + t[1..] : "Event");
                    return new
                    {
                        id = e.Id,
                        title = $"{user.FirstName} \u2014 {label}",
                        eventType = e.EventType,
                        userId = e.UserId,
                        orderId = e.OrderId,
                        start = e.Start.ToString("yyyy-MM-dd"),
                        end = e.End.ToString("yyyy-MM-dd")
                    };
                })
                .ToList();

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> EditEvent([FromBody] EditEventRequest req)
        {
            var ev = await _calendarEventService.GetByIdAsync(req.Id);
            if (ev == null) return Json(new { success = false, error = "Event not found" });

            if (req.Start.HasValue) ev.Start = req.Start.Value;
            if (req.End.HasValue) ev.End = req.End.Value;

            await _calendarEventService.UpdateAsync(ev);

            if (!string.IsNullOrEmpty(ev.OrderId) && !string.IsNullOrEmpty(req.OrderStatusOverride))
            {
                var order = await _orderService.GetByIdAsync(ev.OrderId);
                if (order != null && Enum.TryParse<OrderStatus>(req.OrderStatusOverride, ignoreCase: true, out var newStatus))
                {
                    order.Status = newStatus;
                    await _orderService.UpdateAsync(order);
                }
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<JsonResult> DeleteEvent([FromBody] string id)
        {
            await _calendarEventService.DeleteCalendarEventAsync(id);
            return Json(new { success = true });
        }



        [HttpGet]
        public async Task<JsonResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            var result = users
                .Where(u => u.Role == "employee")
                .Select(u => new
                {
                    id = u.Id,
                    name = u.FirstName
                });

            return Json(result);
        }


        [HttpGet]
        public async Task<JsonResult> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            var result = orders.Select(o => new
            {
                id = o.Id,
                name = o.OrderNumber
            });

            return Json(result);
        }
    }
}

public record CreateCalendarEventRequest(
    string UserId,
    string? OrderId,
    string EventType,
    DateTime? Start,
    DateTime? End,
    string? OrderStatusOverride
);

public record EditEventRequest(
    string Id,
    DateTime? Start,
    DateTime? End,
    string? OrderStatusOverride
);
