using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ScrumExtreme.Domain.Attributes;

namespace ScrumExtreme.Domain.Entities;

public enum OrderStatus
{
    Pending,
    Processing,
    Printed,
    Shipped
}

[CollectionName("Orders")]
public class Order : BaseEntity
{
    [BsonElement("orderNumber")]
    public string OrderNumber { get; set; } = string.Empty;

    [BsonElement("orderDate")]
    public DateTime OrderDate { get; set; }


    [BsonElement("customerId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;


    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [BsonElement("assignedWorkerId")]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfNull]
    public string? AssignedWorkerId { get; set; }

    [BsonElement("shippingAddress")]
    public ShippingAddress ShippingAddress { get; set; } = new();

    [BsonElement("totalAmount")]
    public double TotalAmount { get; set; }

    [BsonElement("items")]
    public List<OrderItem> Items { get; set; } = new();
}