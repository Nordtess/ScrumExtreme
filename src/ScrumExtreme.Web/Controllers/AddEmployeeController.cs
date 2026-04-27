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

        var employeesAndAdmins = users.Where(u => u.Role == "employee" || u.Role == "admin");

        return View(employeesAndAdmins);
    }

    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(string id)
    {
        var employee = await _userService.GetByIdAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
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
            Username = model.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Address = model.Address,
            City = model.City,
            PostalCode = model.PostalCode,
            PhoneNumber = model.PhoneNumber,
            Role = model.Role == "admin" ? "admin" : "employee"
        };
        await _userService.CreateUserAsync(user);
        TempData["Success"] = $"Användaren {user.FirstName} {user.LastName} skapades!";
        return RedirectToAction(nameof(Index));
    }
}
