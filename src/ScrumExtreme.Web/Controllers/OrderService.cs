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

    public async Task CreateOrderAsync(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        order.OrderNumber = GenerateOrderNumber();
        order.OrderDate = DateTime.UtcNow;
        order.Status = OrderStatus.Pending;
        order.TotalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);

        await _repository.AddAsync(order);
    }

    public async Task<Order?> GetByIdAsync(string id) =>
        await _repository.GetByIdAsync(id);

    public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
    {
        var all = await _repository.GetAllAsync();
        return all.Where(o => o.UserId == userId);
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}";
    }
}