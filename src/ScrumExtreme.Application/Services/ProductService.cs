using MongoDB.Driver;
using ScrumExtreme.Application.Interfaces;
using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Application.Services;

public class ProductService : IProductService
{
    private readonly IMongoCollection<Product> _products;

    public ProductService(IMongoDatabase database)
    {
        _products = database.GetCollection<Product>("Hats");
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _products.Find(_ => true).ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(string id)
    {
        return await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
    }
}