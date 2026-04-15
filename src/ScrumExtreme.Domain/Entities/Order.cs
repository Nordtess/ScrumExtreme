using MongoDB.Bson;
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
    [BsonElement("orderNumber")]
    public string OrderNumber { get; set; } = string.Empty;

    [BsonElement("orderDate")]
    public DateTime OrderDate { get; set; }

    // Vi mappar databasens "customerId" till koden "UserId"
    // VIKTIGT: Använd "customerId" i BsonElement så det matchar MongoDB Atlas!
    [BsonElement("customerId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    // Här fixar vi status-felet: Vi använder Enum men lagrar som sträng
    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [BsonElement("shippingAddress")]
    public ShippingAddress ShippingAddress { get; set; } = new();

    [BsonElement("totalAmount")]
    public double TotalAmount { get; set; }

    [BsonElement("items")]
    public List<OrderItem> Items { get; set; } = new();
}