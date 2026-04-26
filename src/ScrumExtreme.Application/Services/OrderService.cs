using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Domain.Interfaces;

namespace ScrumExtreme.Application.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _repository;
    private readonly ISalesStatisticsService _salesStatisticsService;

    public OrderService(IRepository<Order> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync() =>
        await _repository.GetAllAsync();

    public async Task CreateOrderAsync(Order order)
    {
        await _repository.AddAsync(order);

        try
        {
            await _salesStatisticsService.UpdateStatisticsAsync(
                Convert.ToDecimal(order.TotalAmount),
                DateTime.UtcNow   // ? 
            );
        }
        catch
        {
            // påverkar inte resten
        }
    }
    public async Task<Order?> GetByIdAsync(string id) =>
        await _repository.GetByIdAsync(id);

    public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
    {
        var all = await _repository.GetAllAsync();
        return all.Where(o => o.UserId == userId);
    }

    public async Task DeleteOrderAsync(string id) =>
        await _repository.DeleteAsync(id);

    public async Task<IEnumerable<Order>> GetPendingOrdersAsync()
    {
        var all = await _repository.GetAllAsync();
        return all.Where(o => o.Status == OrderStatus.Pending);
    }

    public OrderService(
    IRepository<Order> repository,
    ISalesStatisticsService salesStatisticsService)
    {
        _repository = repository;
        _salesStatisticsService = salesStatisticsService;
    }
    public async Task UpdateAsync(Order order)
    {
        await _repository.UpdateAsync(order.Id, order);

    }
}