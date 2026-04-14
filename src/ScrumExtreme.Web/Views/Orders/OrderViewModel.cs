using System.ComponentModel.DataAnnotations;

namespace ScrumExtreme.Web.Models;

public class CreateOrderViewModel
{
    [Required]
    [Display(Name = "Kund")]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Produktnamn")]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Produkt-ID")]
    public string ProductId { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    [Display(Name = "Antal")]
    public int Quantity { get; set; }

    [Range(0.01, double.MaxValue)]
    [Display(Name = "Styckpris")]
    public decimal UnitPrice { get; set; }
}