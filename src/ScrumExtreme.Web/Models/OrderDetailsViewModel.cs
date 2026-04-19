using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Web.Models
{
    public class OrderDetailsViewModel
    {
        public Order Order { get; set; }
        public string CustomerEmail { get; set; }
    }
}
