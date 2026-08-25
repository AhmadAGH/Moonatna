using Moonatna.Models;

namespace Moonatna.ViewModels.Items;

public class AddItemViewModel
{
    public string Name { get; set; } = string.Empty;
    public bool IsAdHoc { get; set; }
    public int? CategoryId { get; set; }
}

public class SetItemStateViewModel
{
    public int ItemId { get; set; }
    public ItemState State { get; set; }
}

public class SetItemCategoryViewModel
{
    public int ItemId { get; set; }
    public int? CategoryId { get; set; }
}

public class PurchaseItemViewModel
{
    public int ItemId { get; set; }
}
