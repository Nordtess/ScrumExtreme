using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace ScrumExtreme.Domain.Entities;

[BsonIgnoreExtraElements]
public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("price")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Price { get; set; }

    [BsonElement("size")]
    public string Size { get; set; } = string.Empty;
}