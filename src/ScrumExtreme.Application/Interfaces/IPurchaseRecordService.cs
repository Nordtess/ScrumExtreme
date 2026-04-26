using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Application.Interfaces;

public interface IPurchaseRecordService
{
    Task<IEnumerable<PurchaseRecord>> GetAllAsync();
    Task<PurchaseRecord?> GetByIdAsync(string id);
    Task CreateAsync(PurchaseRecord record);
    Task DeleteAsync(string id);
}
