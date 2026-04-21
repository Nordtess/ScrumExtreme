using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ScrumExtreme.Domain.Attributes;

namespace ScrumExtreme.Domain.Entities;

public enum OrderStatus
{
    Pending,
    Processing,
    Printed,
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
    // VIKTIGT: Anv�nd "customerId" i BsonElement s� det matchar MongoDB Atlas!
    [BsonElement("customerId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    // H�r fixar vi status-felet: Vi anv�nder Enum men lagrar som str�ng
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