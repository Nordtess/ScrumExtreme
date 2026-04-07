using MongoDB.Bson.Serialization.Attributes;

namespace ScrumExtreme.Domain.Entities;

public class OrderItem
{
    [BsonElement("productId")] public string ProductId { get; set; } = string.Empty;
    [BsonElement("name")] public string Name { get; set; } = string.Empty;
    [BsonElement("quantity")] public int Quantity { get; set; }
    [BsonElement("unitPrice")] public decimal UnitPrice { get; set; }
}
