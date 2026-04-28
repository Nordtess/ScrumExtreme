namespace ScrumExtreme.Web.Models;

public class KundhanteringViewModel
{
    public int TotalCustomers { get; set; }
    public int CustomersThisMonth { get; set; }
    public int TotalOrders { get; set; }

    public List<RecentCustomerRow> RecentCustomers { get; set; } = [];

    public class RecentCustomerRow
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
    }
}
