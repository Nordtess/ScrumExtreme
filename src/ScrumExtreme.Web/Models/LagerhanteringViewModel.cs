namespace ScrumExtreme.Web.Models;

public class LagerhanteringViewModel
{
    public int TotalHatTypes { get; set; }
    public int TotalHatsInStock { get; set; }
    public int TotalMaterials { get; set; }
    public int TotalItems { get; set; }
    public decimal CapitalSEK { get; set; }

    public int PendingOrderCount { get; set; }
    public int UniqueMaterialsNeeded { get; set; }
    public decimal TotalMaterialCost { get; set; }

    public decimal TotalRevenueLast30Days { get; set; }
    public decimal TotalCostsLast30Days { get; set; }
    public int OrdersLast30Days { get; set; }
}
