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
        private readonly IMaterialService _materialService;
        private readonly IItemService _itemService;

        public SpecialHatsController(
            IOrderService orderService,
            IUserService userService,
            IMaterialService materialService,
            IItemService itemService)
        {
            _orderService = orderService;
            _userService = userService;
            _materialService = materialService;
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            var materials = await _materialService.GetMaterialsAsync();
            var items = await _itemService.GetAllItemsAsync();

            ViewBag.Customers = users.Where(u => u.Role == "customer").ToList();

            return View(new SpecialHatsViewModel
            {
                AvailableMaterials = materials.OrderBy(m => m.Name).ToList(),
                AvailableItems = items.OrderBy(i => i.Name).ToList()
            });
        }

        [HttpPost("Special")]
        public async Task<IActionResult> CreateSpecial(SpecialHatsViewModel model)
        {
            ModelState.Remove(nameof(model.AvailableMaterials));
            ModelState.Remove(nameof(model.AvailableItems));

            if (!ModelState.IsValid)
            {
                var users = await _userService.GetAllUsersAsync();
                var materials = await _materialService.GetMaterialsAsync();
                var items = await _itemService.GetAllItemsAsync();
                ViewBag.Customers = users.Where(u => u.Role == "customer").ToList();
                model.AvailableMaterials = materials.OrderBy(m => m.Name).ToList();
                model.AvailableItems = items.OrderBy(i => i.Name).ToList();
                return View("Index", model);
            }

            var customer = await _userService.GetByIdAsync(model.CustomerId);
            if (customer == null)
                return BadRequest("Kund hittades inte");

            if (model.SelectedMaterials?.Any() == true)
            {
                var allMaterials = await _materialService.GetMaterialsAsync();
                foreach (var name in model.SelectedMaterials)
                {
                    var mat = allMaterials.FirstOrDefault(m =>
                        m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    if (mat != null && mat.Stock > 0)
                    {
                        mat.Stock--;
                        await _materialService.UpdateMaterialAsync(mat);
                    }
                }
            }

            if (model.SelectedItems?.Any() == true)
            {
                var allItems = await _itemService.GetAllItemsAsync();
                foreach (var name in model.SelectedItems)
                {
                    var itm = allItems.FirstOrDefault(i =>
                        i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    if (itm != null && itm.Stock > 0)
                    {
                        itm.Stock--;
                        await _itemService.UpdateItemAsync(itm);
                    }
                }
            }

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
                        Size = model.Size,

                        SpecialHats = new SpecialHats
                        {
                            Name = "Specialbeställd hatt",
                            Sizes = new List<string> { model.Size },
                            MaterialList = string.Join(", ", model.SelectedMaterials ?? new List<string>()),
                            Description = model.Description
                        }
                    }
                },

                TotalAmount = model.Price
            };

            await _orderService.CreateOrderAsync(order);
            TempData["Success"] = "Specialhatt skapad!";
            return RedirectToAction("Index", "Orders");
        }
    }
}

