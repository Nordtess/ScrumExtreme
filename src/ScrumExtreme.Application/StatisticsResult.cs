namespace ScrumExtreme.Application;

public class StatisticsResult
{
    public string Period { get; set; } = "month";
    public decimal TotalRevenue { get; set; }
    public decimal TotalCosts { get; set; }
    public decimal NetProfit { get; set; }
    public int TotalOrders { get; set; }
    public string ChartLabels { get; set; } = "[]";
    public string RevenueData { get; set; } = "[]";
    public string CostData { get; set; } = "[]";
    public string OrderCountData { get; set; } = "[]";
    public int StandardOrders { get; set; }
    public int ModifiedOrders { get; set; }
    public int SpecialOrders { get; set; }
}
