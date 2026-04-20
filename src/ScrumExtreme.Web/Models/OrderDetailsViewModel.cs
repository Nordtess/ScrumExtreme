using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Web.Models
{
    public class OrderDetailsViewModel
    {
        public required Order Order { get; set; }
        public required string CustomerEmail { get; set; }
    }
}
