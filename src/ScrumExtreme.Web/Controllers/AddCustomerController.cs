using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Web.Models;

namespace ScrumExtreme.Web.Controllers;

[Route("AddCustomer")]
public class AddCustomerController : Controller
{
    private readonly IUserService _userService;

    public AddCustomerController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View(new CreateCustomerViewModel());
    }

    [HttpPost("HamtaAllaKunder")]
    public async Task<IActionResult> HamtaAllaKunder()
    {
        var users = await _userService.GetAllUsersAsync();
        ViewBag.Customers = users;
        return View("Index", new CreateCustomerViewModel());
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
        return RedirectToAction(nameof(Index));
    }
}
