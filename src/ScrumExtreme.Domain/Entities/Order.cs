using MongoDB.Bson.Serialization.Attributes;
using ScrumExtreme.Domain.Attributes;

namespace ScrumExtreme.Domain.Entities;

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

[CollectionName("Orders")]
public class Order : BaseEntity
{
    [BsonElement("orderNumber")] public string OrderNumber { get; set; } = string.Empty;
    [BsonElement("orderDate")] public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    [BsonElement("userId")] public string UserId { get; set; } = string.Empty;
    [BsonElement("status")] public OrderStatus Status { get; set; } = OrderStatus.Pending;
    [BsonElement("shippingAddress")] public ShippingAddress ShippingAddress { get; set; } = new();
    [BsonElement("items")] public List<OrderItem> Items { get; set; } = new();
    [BsonElement("totalAmount")] public decimal TotalAmount { get; set; }
}
