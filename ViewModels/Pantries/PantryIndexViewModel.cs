using Moonatna.Models;

namespace Moonatna.ViewModels.Pantries;

public class PantryIndexViewModel
{
    public int FamilyId { get; set; }
    public string FamilyName { get; set; } = string.Empty;
    public IEnumerable<PantryItemViewModel> Items { get; set; } = Enumerable.Empty<PantryItemViewModel>();
}

public class PantryItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ItemState State { get; set; }
    public string? CategoryName { get; set; }
    public string? ImagePath { get; set; }
    public decimal? Quantity { get; set; }
}
