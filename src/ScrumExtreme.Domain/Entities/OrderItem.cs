using MongoDB.Bson.Serialization.Attributes;

namespace ScrumExtreme.Domain.Entities;

public class OrderItem
{
    [BsonElement("productId")]
    public string ProductId { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("unitPrice")]
    public double UnitPrice { get; set; }

    [BsonElement("size")]
    public string Size { get; set; } = string.Empty;

    [BsonElement("isModified")]
    public bool IsModified {  get; set; }

    [BsonElement("modificationDescription")]
    public string ModificationDescription { get; set; } = string.Empty;

    //Ska senare ändras till List<Material> när Material-entiteten finns
    [BsonElement("materialIds")]
    public List<string> MaterialIds { get; set; } = new();

    [BsonElement("addedMaterialCost")] 
    public double AddedMaterialCost { get; set; }

    [BsonElement("extraWorkHours")]
    public double ExtraWorkHours { get; set; }

}