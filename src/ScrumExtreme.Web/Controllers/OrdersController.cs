using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Web.Models;

namespace ScrumExtreme.Web.Controllers;

[Route("Orders")]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IUserService _userService;
    private readonly IHatService _hatService;
    private readonly IItemService _itemService;
    private readonly ICompanySettingsService _companySettingsService;
    private readonly ICalendarEventService _calendarEventService;

    public OrdersController(
        IOrderService orderService,
        IUserService userService,
        IHatService hatService,
        IItemService itemService,
        ICompanySettingsService companySettingsService,
        ICalendarEventService calendarEventService)
    {
        _orderService = orderService;
        _userService = userService;
        _hatService = hatService;
        _itemService = itemService;
        _companySettingsService = companySettingsService;
        _calendarEventService = calendarEventService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        var users = await _userService.GetAllUsersAsync();
        ViewBag.UserLookup = users.ToDictionary(u => u.Id, u => u);
        return View(orders);
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        var users = await _userService.GetAllUsersAsync();
        ViewBag.Customers = users.Where(u => u.Role == "customer");

        var hats = await _hatService.GetAllHatsAsync();
        ViewBag.Hats = hats;

        var items = await _itemService.GetAllItemsAsync();
        ViewBag.Items = items;

        return View();
    }

    [HttpGet("CustomerInfo/{id}")]
    public async Task<IActionResult> CustomerInfo(string id)
    {
        var users = await _userService.GetAllUsersAsync();
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound();

        return Json(new
        {
            fullName = $"{user.FirstName} {user.LastName}",
            address = user.Address,
            city = user.City,
            postalCode = user.PostalCode,
            country = user.Country,
            countryCode = user.CountryCode,
            phone = user.PhoneNumber,
            email = user.Email
        });
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateOrderViewModel model)
    {
        if (string.IsNullOrEmpty(model.CustomerId) || model.Items == null || !model.Items.Any())
            return BadRequest(new { error = "Välj en kund och lägg till minst en hatt." });

        var users = await _userService.GetAllUsersAsync();
        var customer = users.FirstOrDefault(u => u.Id == model.CustomerId);
        if (customer == null)
            return BadRequest(new { error = "Kunden hittades inte." });

        foreach (var item in model.Items)
        {
            if (item.Quantity < 1)
                return BadRequest(new { error = $"Antal för '{item.Name}' ({item.Size}) måste vara minst 1." });

            var hat = await _hatService.GetByIdAsync(item.HatId);
            if (hat == null)
                return BadRequest(new { error = $"Hatten '{item.Name}' hittades inte." });
            if (!string.IsNullOrEmpty(item.Size))
            {
                hat.Stock.TryGetValue(item.Size, out var stock);
                if (stock < item.Quantity)
                    return BadRequest(new { error = $"'{item.Name}' storlek {item.Size}: endast {stock} i lager (du begärde {item.Quantity})." });
            }
        }

        foreach (var hatItem in model.Items.Where(i => i.IsModified && i.ItemIds.Any()))
        {
            foreach (var accessoryId in hatItem.ItemIds)
            {
                var accessory = await _itemService.GetByIdAsync(accessoryId);
                if (accessory == null)
                    return BadRequest(new { error = "Ett tillbehör hittades inte. Försök igen." });
                if (accessory.Stock < hatItem.Quantity)
                    return BadRequest(new { error = $"'{accessory.Name}': endast {accessory.Stock} i lager (du behöver {hatItem.Quantity})." });
            }
        }

        var orderNumber = $"ORD-{DateTime.UtcNow.Year}-{Random.Shared.Next(1000, 9999)}";

        var order = new Order
        {
            OrderNumber = orderNumber,
            UserId = customer.Id,
            OrderDate = DateTime.UtcNow,
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
            Items = model.Items.Select(i => new OrderItem
            {
                ProductId = i.HatId,
                Name = i.Name,
                Size = i.Size,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,

                IsModified = i.IsModified,
                ModificationDescription = i.ModificationDescription,
                ItemIds = i.ItemIds,
                AddedMaterialCost = 0
            }).ToList(),
            TotalAmount = model.Items.Sum(i => i.UnitPrice * i.Quantity)
        };

        await _orderService.CreateOrderAsync(order);

        await _companySettingsService.AddCapitalAsync((decimal)order.TotalAmount);

        foreach (var item in order.Items)
        {
            var hat = await _hatService.GetByIdAsync(item.ProductId);
            if (hat != null && !string.IsNullOrEmpty(item.Size))
            {
                hat.Stock.TryGetValue(item.Size, out var current);
                hat.Stock[item.Size] = Math.Max(0, current - item.Quantity);
                await _hatService.UpdateHatAsync(hat);
            }
        }

        foreach (var hatItem in order.Items.Where(i => i.IsModified && i.ItemIds.Any()))
        {
            foreach (var accessoryId in hatItem.ItemIds)
            {
                var accessory = await _itemService.GetByIdAsync(accessoryId);
                if (accessory != null)
                {
                    accessory.Stock = Math.Max(0, accessory.Stock - hatItem.Quantity);
                    await _itemService.UpdateItemAsync(accessory);
                }
            }
        }

        return Ok(new { success = true });
    }

    [HttpPost("Delete/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null) return NotFound();

        await _orderService.DeleteOrderAsync(id);
        return Ok(new { success = true });
    }

    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(string id)
    {
        var order = await _orderService.GetByIdAsync(id);

        if (order == null)
        {
            return NotFound();
        }
        var user = await _userService.GetByIdAsync(order.UserId);

        var allItems = await _itemService.GetAllItemsAsync();
        var materialDict = allItems.ToDictionary(i => i.Id, i => i.Name);

        string? assignedWorkerName = null;
        if (!string.IsNullOrEmpty(order.AssignedWorkerId))
        {
            var worker = await _userService.GetByIdAsync(order.AssignedWorkerId);
            if (worker != null)
                assignedWorkerName = $"{worker.FirstName} {worker.LastName}";
        }

        ViewBag.UserRole = HttpContext.Session.GetString("UserRole") ?? "";
        ViewBag.UserId = HttpContext.Session.GetString("UserId") ?? "";

        var viewModel = new OrderDetailsViewModel
        {
            Order = order,
            CustomerEmail = user?.Email ?? "Okänd kund",
            MaterialNames = materialDict,
            AssignedWorkerName = assignedWorkerName
        };
        return View(viewModel);
    }

    [HttpPost("StartOrder/{id}")]
    public async Task<IActionResult> StartOrder(string id, [FromBody] StartOrderRequest request)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null) return NotFound();
        if (order.Status != OrderStatus.Pending)
            return BadRequest(new { error = "Ordern är inte i status Väntar." });

        var workerId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(workerId))
            return Unauthorized();

        order.Status = OrderStatus.Processing;
        order.AssignedWorkerId = workerId;
        await _orderService.UpdateAsync(order);

        var calEvent = new CalendarEvent
        {
            UserId = workerId,
            OrderId = order.Id,
            Start = DateTime.UtcNow,
            End = request.EstimatedEnd.ToUniversalTime()
        };
        await _calendarEventService.CreateEventAsync(calEvent);

        return Ok(new { success = true });
    }

    [HttpPost("CompleteOrder/{id}")]
    public async Task<IActionResult> CompleteOrder(string id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null) return NotFound();
        if (order.Status != OrderStatus.Processing)
            return BadRequest(new { error = "Ordern är inte i status Påbörjad." });

        order.Status = OrderStatus.Printed;
        await _orderService.UpdateAsync(order);
        return Ok(new { success = true });
    }

    [HttpPost("ShipOrder/{id}")]
    public async Task<IActionResult> ShipOrder(string id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null) return NotFound();
        if (order.Status != OrderStatus.Printed)
            return BadRequest(new { error = "Ordern är inte i status Utskriven." });

        if (!string.IsNullOrEmpty(order.AssignedWorkerId))
        {
            var workerEvents = await _calendarEventService.GetByUserIdAsync(order.AssignedWorkerId);
            var orderEvent = workerEvents.FirstOrDefault(e => e.OrderId == order.Id);
            if (orderEvent != null)
                await _calendarEventService.DeleteCalendarEventAsync(orderEvent.Id);
        }

        order.Status = OrderStatus.Shipped;
        order.AssignedWorkerId = null;
        await _orderService.UpdateAsync(order);
        return Ok(new { success = true });
    }
}

public record StartOrderRequest(DateTime EstimatedEnd);
