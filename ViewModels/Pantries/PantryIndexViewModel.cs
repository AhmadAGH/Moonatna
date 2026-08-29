using Moonatna.Models;

namespace Moonatna.ViewModels.Pantries;

public class PantryIndexViewModel
{
    public int FamilyId { get; set; }
    public string FamilyName { get; set; } = string.Empty;
    public List<PantryItemViewModel> Items { get; set; } = new();
}

public class PantryItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ItemState State { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryIcon { get; set; }
    public string? ImagePath { get; set; }
    public int? Quantity { get; set; }


    // Localization key for step 8: "ItemState.Available" etc.
    public string StateKey => $"ItemState.{State}";
}
