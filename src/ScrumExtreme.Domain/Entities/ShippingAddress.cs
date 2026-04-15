using MongoDB.Bson.Serialization.Attributes;

namespace ScrumExtreme.Domain.Entities;

public class ShippingAddress
{
    [BsonElement("fullName")] public string FullName { get; set; } = string.Empty;
    [BsonElement("address")] public string Address { get; set; } = string.Empty;
    [BsonElement("city")] public string City { get; set; } = string.Empty;
    [BsonElement("postalCode")] public string PostalCode { get; set; } = string.Empty;
    [BsonElement("country")] public string Country { get; set; } = string.Empty;
    [BsonElement("countryCode")] public string CountryCode { get; set; } = string.Empty;
    [BsonElement("phone")] public string Phone { get; set; } = string.Empty;
}


