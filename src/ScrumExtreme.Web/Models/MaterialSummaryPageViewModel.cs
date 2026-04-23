namespace ScrumExtreme.Web.Models
{
    public class MaterialSummaryPageViewModel
    {
        public List<MaterialSummaryViewModel> Materials { get; set; } = new();
        public List<ItemSummaryViewModel> Items { get; set; } = new();
        public decimal CapitalSEK { get; set; }
    }
}
