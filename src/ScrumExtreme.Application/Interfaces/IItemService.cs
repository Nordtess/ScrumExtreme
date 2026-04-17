using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Application.Interfaces;

public interface IItemService
{
    Task<IEnumerable<Item>> GetAllItemsAsync();
    Task<Item?> GetByIdAsync(string id);
    Task CreateItemAsync(Item item);
    Task UpdateItemAsync(Item item);
    Task DeleteItemAsync(string id);
}
