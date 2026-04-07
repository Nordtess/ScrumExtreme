using Microsoft.AspNetCore.Mvc;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Web.Models;

namespace ScrumExtreme.Web.Controllers;

public class AdminController : Controller
{
    private readonly ICustomerService _customerService;

    public AdminController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new CreateCustomerViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> HamtaAllaKunder()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        ViewBag.Customers = customers;
        return View("Index", new CreateCustomerViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> SkapaKund(CreateCustomerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var customer = new Customer
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Address = model.Address,
            City = model.City,
            PostalCode = model.PostalCode,
            Country = model.Country,
            PhoneNumber = model.PhoneNumber
        };

        await _customerService.CreateCustomerAsync(customer);
        TempData["Success"] = $"Kunden {customer.FirstName} {customer.LastName} skapades!";
        return RedirectToAction(nameof(Index));
    }
}
