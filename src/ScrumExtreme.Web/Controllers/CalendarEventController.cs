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
                .Select(e => e.UserId)
                .Distinct()
                .ToList();
            var workerColors = workerIds
                .Select((id, i) => (id, color: palette[i % palette.Length]))
                .ToDictionary(x => x.id, x => x.color);

            var result = events
                .Where(e => userDict.ContainsKey(e.UserId) && orderDict.ContainsKey(e.OrderId))
                .Select(e => new
                {
                    id = e.Id,
                    title = $"{userDict[e.UserId].FirstName} \u2014 {orderDict[e.OrderId].OrderNumber}",
                    start = e.Start,
                    end = e.End,
                    color = workerColors.TryGetValue(e.UserId, out var c) ? c : palette[0],
                    extendedProps = new
                    {
                        orderId = e.OrderId,
                        orderNumber = orderDict[e.OrderId].OrderNumber,
                        workerName = $"{userDict[e.UserId].FirstName} {userDict[e.UserId].LastName}"
                    }
                });

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> CreateEvent([FromBody] CalendarEvent model)
        {
            await _calendarEventService.CreateEventAsync(model);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<JsonResult> UpdateEvent([FromBody] CalendarEvent model)
        {
            await _calendarEventService.UpdateAsync(model);
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
