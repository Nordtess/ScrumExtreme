namespace ScrumExtreme.Web.Models;

public class ShippingLabelViewModel
{
    public bool HasOrder { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
}
