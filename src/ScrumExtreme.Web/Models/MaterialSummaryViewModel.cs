namespace ScrumExtreme.Web.Models
{
    public class MaterialSummaryViewModel
    {
        public string MaterialName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice => PricePerUnit * TotalQuantity;
    }
}