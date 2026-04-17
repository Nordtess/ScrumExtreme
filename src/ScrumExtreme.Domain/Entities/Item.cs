using MongoDB.Bson.Serialization.Attributes;
using ScrumExtreme.Domain.Attributes;

namespace ScrumExtreme.Domain.Entities;

[CollectionName("Items")]
[BsonIgnoreExtraElements]
public class Item : BaseEntity
{
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("price")]
    public double Price { get; set; }

    [BsonElement("stock")]
    public int Stock { get; set; }
}
