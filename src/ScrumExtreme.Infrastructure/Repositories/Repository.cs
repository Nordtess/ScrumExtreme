using MongoDB.Bson;
using MongoDB.Driver;
using ScrumExtreme.Domain.Attributes;
using ScrumExtreme.Domain.Interfaces;

namespace ScrumExtreme.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly IMongoCollection<T> _collection;

    public Repository(IMongoDatabase database)
    {
        var collectionAttr = typeof(T)
            .GetCustomAttributes(typeof(CollectionNameAttribute), false)
            .FirstOrDefault() as CollectionNameAttribute;

        var collectionName = collectionAttr?.Name ?? typeof(T).Name;
        _collection = database.GetCollection<T>(collectionName);
    }

    // MongoDB stores _id as ObjectId; we must parse the string to ObjectId when filtering.
    private static FilterDefinition<T> ById(string id)
    {
        if (ObjectId.TryParse(id, out var oid))
            return Builders<T>.Filter.Eq("_id", oid);
        return Builders<T>.Filter.Eq("_id", id);
    }

    public async Task<T?> GetByIdAsync(string id) =>
        await _collection.Find(ById(id)).FirstOrDefaultAsync();

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task AddAsync(T entity) =>
        await _collection.InsertOneAsync(entity);

    public async Task UpdateAsync(string id, T entity) =>
        await _collection.ReplaceOneAsync(ById(id), entity);

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(ById(id));
}
