using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Web.Models
{
    public class SpecialHatsViewModel
    {
        [Required(ErrorMessage = "En storlek måste väljas.")]
        public string Size { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pris är obligatoriskt.")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Beskrivning är obligatoriskt.")]
        public string Description { get; set; } = string.Empty;

        public string CustomerId { get; set; } = string.Empty;

        public List<string> SelectedMaterials { get; set; } = new();
        public List<string> SelectedItems { get; set; } = new();

        // Populated in GET for rendering checkboxes
        public List<Material> AvailableMaterials { get; set; } = new();
        public List<Item> AvailableItems { get; set; } = new();
    }
}
