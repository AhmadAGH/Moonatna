using Moonatna.Models;

namespace Moonatna.ViewModels.Shopping;

public class ShoppingListViewModel
{
    public int FamilyId { get; set; }
    public string FamilyName { get; set; } = string.Empty;
    public List<ShoppingItemViewModel> Items { get; set; } = new();
}

public class ShoppingItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ItemState State { get; set; }        // LowStock or OutOfStock only
    public bool IsAdHoc { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryIcon { get; set; }
    public string? ImagePath { get; set; }
    public int? Quantity { get; set; }

    public string StateKey => $"ItemState.{State}";
}
