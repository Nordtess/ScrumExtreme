using MongoDB.Bson.Serialization.Attributes;
using ScrumExtreme.Domain.Attributes;

namespace ScrumExtreme.Domain.Entities;

[CollectionName("CompanySettings")]
[BsonIgnoreExtraElements]
public class CompanySettings : BaseEntity
{
    [BsonElement("capitalSEK")]
    public decimal CapitalSEK { get; set; }

    [BsonElement("lastUpdated")]
    public DateTime LastUpdated { get; set; }
}
