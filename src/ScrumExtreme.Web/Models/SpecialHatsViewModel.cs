using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ScrumExtreme.Web.Models
{
    public class SpecialHatsViewModel
    {
        [Required(ErrorMessage = "Namn är obligatoriskt.")]
        public string Name { get; set; } = string.Empty;

        [MinLength(1, ErrorMessage = "Minst en storlek måste väljas.")]
        public List<string> Sizes { get; set; } = new();

        [Required(ErrorMessage = "Pris är obligatoriskt.")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Materiallista är obligatoriskt.")]
        public string MaterialList { get; set; } = string.Empty;

        [Required(ErrorMessage = "Beskrivning är obligatoriskt.")]
        public string Description { get; set; } = string.Empty;

        public string CustomerId { get; set; } = string.Empty;
    }
}
