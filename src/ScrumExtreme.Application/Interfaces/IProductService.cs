using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetByIdAsync(string id);
}