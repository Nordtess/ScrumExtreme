namespace ScrumExtreme.Web.Models;

public class OrderhanteringViewModel
{
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int ProcessingOrders { get; set; }
    public int PrintedOrders { get; set; }
    public int ShippedOrders { get; set; }

    public int TotalCustomers { get; set; }
    public int TotalHatModels { get; set; }

    public List<RecentOrderRow> RecentOrders { get; set; } = [];

    public class RecentOrderRow
    {
        public string OrderId { get; set; } = "";
        public string OrderNumber { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string StatusLabel { get; set; } = "";
        public string StatusColor { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
    }
}
