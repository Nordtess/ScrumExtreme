using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Web.Models;

namespace ScrumExtreme.Web.Controllers;

[Route("AddEmployee")]
public class AddEmployeeController : Controller
{
    private readonly IUserService _userService;

    public AddEmployeeController(IUserService userService)
        {
        _userService = userService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View(new CreateEmployeeViewModel());
    }

    [HttpGet("AllEmployees")]
    public async Task<IActionResult> AllEmployees()
    {
        var users = await _userService.GetAllUsersAsync();
        return View(users.Where(u => u.Role == "employee"));
    }

    [HttpPost("SkapaAnstalld")]
    public async Task<IActionResult> SkapaAnstalld(CreateEmployeeViewModel model)
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
            PhoneNumber = model.PhoneNumber,
            Role = "employee"
        };
        await _userService.CreateUserAsync(user);
        TempData["Success"] = $"Användaren {user.FirstName} {user.LastName} skapades!";
        return RedirectToAction(nameof(Index));
    }
}
