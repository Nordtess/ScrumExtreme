using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Domain.Interfaces;

namespace ScrumExtreme.Web.Controllers
{
    public class CalendarController : Controller
    {
        private readonly IRepository<CalendarEvent> _events;
        private readonly IRepository<User> _users;
        private readonly IRepository<Order> _orders;

        public CalendarController(
            IRepository<CalendarEvent> events,
            IRepository<User> users,
            IRepository<Order> orders)
        {
            _events = events;
            _users = users;
            _orders = orders;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetEvents()
        {
            var events = await _events.GetAllAsync();
            var users = await _users.GetAllAsync();
            var orders = await _orders.GetAllAsync();

            var userDict = users.ToDictionary(u => u.Id);
            var orderDict = orders.ToDictionary(o => o.Id);

            var result = events.Select(e => new
            {
                id = e.Id,
                title = $"{orderDict[e.OrderId].OrderNumber} - {userDict[e.UserId].FirstName}",
                start = e.Start,
                end = e.End
            });

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> CreateEvent([FromBody] CalendarEvent model)
        {
            await _events.AddAsync(model);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<JsonResult> UpdateEvent([FromBody] CalendarEvent model)
        {
            await _events.UpdateAsync(model.Id, model);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<JsonResult> DeleteEvent(string id)
        {
            await _events.DeleteAsync(id);
            return Json(new { success = true });
        }
    }
}
