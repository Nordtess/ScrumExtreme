using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;
using ScrumExtreme.Domain.Interfaces;

namespace ScrumExtreme.Application.Services;

public class PurchaseRecordService : IPurchaseRecordService
{
    private readonly IRepository<PurchaseRecord> _repository;

    public PurchaseRecordService(IRepository<PurchaseRecord> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PurchaseRecord>> GetAllAsync() =>
        await _repository.GetAllAsync();

    public async Task<PurchaseRecord?> GetByIdAsync(string id) =>
        await _repository.GetByIdAsync(id);

    public async Task CreateAsync(PurchaseRecord record) =>
        await _repository.AddAsync(record);

    public async Task DeleteAsync(string id) =>
        await _repository.DeleteAsync(id);
}
