using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Domain.Interfaces;

namespace ScrumExtreme.Application.Services;

public class ItemService : IItemService
{
    private readonly IRepository<Item> _repository;

    public ItemService(IRepository<Item> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Item>> GetAllItemsAsync() =>
        await _repository.GetAllAsync();

    public async Task<Item?> GetByIdAsync(string id) =>
        await _repository.GetByIdAsync(id);

    public async Task CreateItemAsync(Item item) =>
        await _repository.AddAsync(item);

    public async Task UpdateItemAsync(Item item) =>
        await _repository.UpdateAsync(item.Id, item);

    public async Task DeleteItemAsync(string id) =>
        await _repository.DeleteAsync(id);
}
