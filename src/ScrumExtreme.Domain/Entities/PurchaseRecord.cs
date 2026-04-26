using MongoDB.Bson.Serialization.Attributes;
using ScrumExtreme.Domain.Attributes;

namespace ScrumExtreme.Domain.Entities;

[CollectionName("PurchaseRecords")]
public class PurchaseRecord : BaseEntity
{
    [BsonElement("type")]
    public string Type { get; set; } = string.Empty;

    [BsonElement("referenceId")]
    public string ReferenceId { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("unitCost")]
    public decimal UnitCost { get; set; }

    [BsonElement("totalCost")]
    public decimal TotalCost { get; set; }

    [BsonElement("purchasedAt")]
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
}
