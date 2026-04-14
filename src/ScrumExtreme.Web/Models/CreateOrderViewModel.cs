using System.ComponentModel.DataAnnotations;

namespace ScrumExtreme.Web.Models;

public class CreateOrderViewModel
{
    [Required]
    public string CustomerId { get; set; } = string.Empty;

    [Required]
    public string ProductId { get; set; } = string.Empty;
}