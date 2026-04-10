using System.ComponentModel.DataAnnotations;

namespace ScrumExtreme.Web.Models
{
    public class CreateHatsViewModel
    {
        [Required(ErrorMessage = "Namn är obligatoriskt.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Storlek är obligatoriskt.")]
        public string Size { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pris är obligatoriskt.")]
        public double Price { get; set; } 

        [Required(ErrorMessage = "Materiallista är obligatoriskt.")]
        public string MaterialList { get; set; } = string.Empty;

        
    }
}
