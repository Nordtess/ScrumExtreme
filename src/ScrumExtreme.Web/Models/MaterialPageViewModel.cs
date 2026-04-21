using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Web.Models
{
    public class MaterialPageViewModel
    {
        public CreateMaterialViewModel NewMaterial { get; set; } = new();
        public IEnumerable<Material> Materials { get; set; } = new List<Material>();
        public string Search { get; set; } = string.Empty;
    }
}