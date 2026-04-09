using MongoDB.Bson.Serialization.Attributes;
using ScrumExtreme.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumExtreme.Domain.Entities
{
    [CollectionName("Hats")]
    public class Hats : BaseEntity
    {
        [BsonElement("name")] public string Name { get; set; } = string.Empty;

        [BsonElement("size")] public string Size { get; set; } = string.Empty;

        [BsonElement("price")] public double Price { get; set; }

        [BsonElement("materiallist")] public string MaterialList { get; set; } = string.Empty;

        
    }
}
