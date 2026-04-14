using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Application.Services;

public class ProductService : IProductService
{
    private static readonly List<Product> _products = new()
    {
        new Product { Id = "1", Name = "T-shirt", Price = 199m },
        new Product { Id = "2", Name = "Keps", Price = 149m },
        new Product { Id = "3", Name = "Hoodie", Price = 499m }
    };

    public Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return Task.FromResult<IEnumerable<Product>>(_products);
    }

    public Task<Product?> GetByIdAsync(string id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);
    }
}