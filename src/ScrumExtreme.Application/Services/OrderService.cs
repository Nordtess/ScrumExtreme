using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Domain.Interfaces;

namespace ScrumExtreme.Application.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _repository;

    public OrderService(IRepository<Order> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync() =>
        await _repository.GetAllAsync();

    public async Task CreateOrderAsync(Order order) =>
        await _repository.AddAsync(order);

    public async Task<Order?> GetByIdAsync(string id) =>
        await _repository.GetByIdAsync(id);

    // Ändrat namn från GetByCustomerIdAsync till GetByUserIdAsync 
    // för att matcha IOrderService exakt!
    public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
    {
        var all = await _repository.GetAllAsync();
        // Ändra CustomerId till UserId här!
        return all.Where(o => o.UserId == userId);
        // Här mappar vi mot UserId i din Order-entitet. 
        // (Se till att Order-klassen har egenskapen UserId)
        return all.Where(o => o.UserId == userId);
    }
}