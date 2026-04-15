namespace ScrumExtreme.Web.Models;

public class OrderItemInput
{
    public string HatId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double UnitPrice { get; set; }
}

public class CreateOrderViewModel
{
    public string CustomerId { get; set; } = string.Empty;
    public List<OrderItemInput> Items { get; set; } = new();
}