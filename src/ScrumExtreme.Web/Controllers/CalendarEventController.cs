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

            var result = events.Select(e => new
            {
                id = e.Id,
                //title = $"{orderDict[e.OrderId].OrderNumber} - {userDict[e.UserId].FirstName}",

                start = e.Start,
                end = e.End
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
        public async Task<JsonResult> DeleteEvent(string id)
        {
            await _calendarEventService.DeleteCalendarEventAsync(id);
            return Json(new { success = true });
        }



        [HttpGet]
        public async Task<JsonResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            var result = users.Select(u => new
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
