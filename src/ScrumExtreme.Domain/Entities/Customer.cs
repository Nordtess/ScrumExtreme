using MongoDB.Bson.Serialization.Attributes;
using ScrumExtreme.Domain.Attributes;

namespace ScrumExtreme.Domain.Entities;

[CollectionName("Customers")]

public class Customer : BaseEntity
{
    [BsonElement("firstname")]
    public string FirstName { get; set; } = string.Empty;

    [BsonElement("lastname")]
    public string LastName { get; set; } = string.Empty;

    [BsonElement("address")]
    public string Address { get; set; } = string.Empty;

    [BsonElement("city")]
    public string City { get; set; } = string.Empty;

    [BsonElement("postalCode")]
    public string PostalCode { get; set; } = string.Empty;

    [BsonElement("country")]
    public string Country { get; set; } = string.Empty;

    [BsonElement("phonenumber")]
    public string PhoneNumber { get; set; } = string.Empty;
}
