using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task CreateOrderAsync(Order order);
    Task<Order?> GetByIdAsync(string id);
    Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
    Task UpdateOrderAsync(Order order);
}
