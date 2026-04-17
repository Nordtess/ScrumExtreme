using ScrumExtreme.Domain.Entities;

namespace ScrumExtreme.Web.Models;

public class WarehouseViewModel
{
    public IEnumerable<Hats> Hats { get; set; } = Enumerable.Empty<Hats>();
    public IEnumerable<Item> Items { get; set; } = Enumerable.Empty<Item>();
}
