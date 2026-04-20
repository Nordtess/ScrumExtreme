using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Web.Models;

namespace ScrumExtreme.Web.Controllers
{
    public class SpecialHatsController : Controller
    {

        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public SpecialHatsController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        [HttpGet]
        public async Task <IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();

            ViewBag.Customers = users.Where(u => !u.IsAdmin).ToList();

            return View(new SpecialHatsViewModel());
        }
    

    [HttpPost("Special")]
        public async Task<IActionResult> CreateSpecial(SpecialHatsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var users = await _userService.GetAllUsersAsync();
                ViewBag.Customers = users.Where(u => !u.IsAdmin).ToList();
                return View(model);
            }

            var customer = await _userService.GetByIdAsync(model.CustomerId);

            if (customer == null)
                return BadRequest("Kund hittades inte");

            var order = new Order
            {
                OrderNumber = $"ORD-{DateTime.UtcNow.Year}-{Random.Shared.Next(1000, 9999)}",
                OrderDate = DateTime.UtcNow,
                UserId = customer.Id,
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

                Items = new List<OrderItem>
        {
            new OrderItem
            {
                Name = "Specialbeställd hatt",
                Quantity = 1,
                UnitPrice = model.Price,

                SpecialHats = new SpecialHats
                {
                    Name = model.Name,
                    Sizes = model.Sizes,
                    MaterialList = model.MaterialList,
                    Description = model.Description
                }
            }
        },

                TotalAmount = model.Price
            };

            await _orderService.CreateOrderAsync(order);

            return RedirectToAction("Index", "Orders");
        }
    }
}




