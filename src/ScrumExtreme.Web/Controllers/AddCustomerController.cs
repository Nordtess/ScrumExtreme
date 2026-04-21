using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Web.Models;

[Route("AddCustomer")]
public class AddCustomerController : Controller
{
    private readonly IUserService _userService;
    private readonly IOrderService _orderService;

    public AddCustomerController(IUserService userService, IOrderService orderService)
    {
        _userService = userService;
        _orderService = orderService;
    }

    [HttpGet("")]
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View(new CreateCustomerViewModel());
    }

    [HttpGet("AllCustomers")]
    public async Task<IActionResult> AllCustomers()
    {
        var users = await _userService.GetAllUsersAsync();
        return View(users);
    }

    [HttpPost("HamtaAllaKunder")]
    public IActionResult HamtaAllaKunder()
    {
        return RedirectToAction(nameof(AllCustomers));
    }

    [HttpPost("SkapaKund")]
    public async Task<IActionResult> SkapaKund(CreateCustomerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Address = model.Address,
            City = model.City,
            PostalCode = model.PostalCode,
            CountryCode = model.CountryCode,
            Country = model.Country,
            PhoneNumber = model.PhoneNumber
        };

        await _userService.CreateUserAsync(user);
        TempData["Success"] = $"Användaren {user.FirstName} {user.LastName} skapades!";
        return RedirectToAction(nameof(AllCustomers));
    }

    // Ny kod
    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(string id)
    {
        var customer = await _userService.GetByIdAsync(id);

        if (customer == null)
            return NotFound();

        var orders = await _orderService.GetByUserIdAsync(id);
        ViewBag.Orders = orders;

        return View(customer);
    }
}

