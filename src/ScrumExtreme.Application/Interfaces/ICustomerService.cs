using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Application.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<Customer>> GetAllCustomersAsync();
    Task CreateCustomerAsync(Customer customer);
}
