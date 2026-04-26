using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ScrumExtreme.Domain.Entities;

public class SalesStatistic
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string PeriodType { get; set; } = null!;
    public int Year { get; set; }
    public int? Month { get; set; }
    public int? Quarter { get; set; }

    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}