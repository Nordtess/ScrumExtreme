using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Domain.Interfaces;

namespace ScrumExtreme.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IRepository<Customer> _repository;

    public CustomerService(IRepository<Customer> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync() =>
        await _repository.GetAllAsync();

    public async Task CreateCustomerAsync(Customer customer) =>
        await _repository.AddAsync(customer);
}
