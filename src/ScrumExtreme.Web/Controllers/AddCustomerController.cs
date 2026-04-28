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
        return View(users.Where(u => u.Role == "customer"));
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

    [HttpDelete("DeleteCustomer/{id}")]
    public async Task<IActionResult> DeleteCustomer(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest();

        var customer = await _userService.GetByIdAsync(id);
        if (customer == null)
            return NotFound();

        await _userService.DeleteUserAsync(id);
        return Ok();
    }

    [HttpPost("UpdateCustomer/{id}")]
    public async Task<IActionResult> UpdateCustomer(string id, [FromBody] UpdateCustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(id) || request == null)
            return BadRequest();

        var customer = await _userService.GetByIdAsync(id);
        if (customer == null)
            return NotFound();

        customer.FirstName = request.FirstName?.Trim() ?? customer.FirstName;
        customer.LastName = request.LastName?.Trim() ?? customer.LastName;
        customer.Email = request.Email?.Trim().ToLowerInvariant() ?? customer.Email;
        customer.Address = request.Address?.Trim() ?? customer.Address;
        customer.PostalCode = request.PostalCode?.Trim() ?? customer.PostalCode;
        customer.City = request.City?.Trim() ?? customer.City;
        customer.Country = request.Country?.Trim() ?? customer.Country;
        customer.CountryCode = request.CountryCode?.Trim() ?? customer.CountryCode;
        customer.PhoneNumber = request.PhoneNumber?.Trim() ?? customer.PhoneNumber;

        await _userService.UpdateUserAsync(id, customer);
        return Ok();
    }
}

public record UpdateCustomerRequest(
    string? FirstName,
    string? LastName,
    string? Email,
    string? Address,
    string? PostalCode,
    string? City,
    string? Country,
    string? CountryCode,
    string? PhoneNumber
);

