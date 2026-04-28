namespace ScrumExtreme.Web.Models;

public class MittArbeteViewModel
{
    public string WorkerName { get; set; } = "";

    public int ActiveOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int UpcomingEvents { get; set; }

    public List<MyOrderRow> MyOrders { get; set; } = [];
    public List<MyEventRow> MyEvents { get; set; } = [];

    public class MyOrderRow
    {
        public string OrderId { get; set; } = "";
        public string OrderNumber { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string StatusLabel { get; set; } = "";
        public string StatusColor { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
    }

    public class MyEventRow
    {
        public string TypeLabel { get; set; } = "";
        public string TypeColor { get; set; } = "";
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
