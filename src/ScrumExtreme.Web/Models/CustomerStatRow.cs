namespace ScrumExtreme.Web.Models;

public class CustomerStatRow
{
    public string CustomerId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public double TotalSpent { get; set; }
    public string TopHat { get; set; } = string.Empty;
    public List<OrderHistoryRow> Orders { get; set; } = new();
}

public class OrderHistoryRow
{
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public double Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
}

public class HatStatRow
{
    public string HatName { get; set; } = string.Empty;
    public int TotalOrders { get; set; }
    public int TotalUnits { get; set; }
    public double TotalRevenue { get; set; }
}
