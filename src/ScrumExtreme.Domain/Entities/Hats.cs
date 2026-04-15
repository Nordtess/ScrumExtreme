using MongoDB.Bson.Serialization.Attributes;
using ScrumExtreme.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumExtreme.Domain.Entities
{
    [CollectionName("Hats")]
    [BsonIgnoreExtraElements]
    public class Hats : BaseEntity
    {
        [BsonElement("name")] public string Name { get; set; } = string.Empty;

        [BsonElement("sizes")] public List<string> Sizes { get; set; } = new();

        [BsonElement("price")] public double Price { get; set; }

        [BsonElement("materiallist")] public string MaterialList { get; set; } = string.Empty;

        [BsonElement("stock")] public Dictionary<string, int> Stock { get; set; } = new();
    }
}
