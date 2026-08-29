using System.ComponentModel.DataAnnotations;
using Moonatna.Models;

namespace Moonatna.ViewModels.Items;

public class AddItemViewModel
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    public int? CategoryId { get; set; }
    public bool IsAdHoc { get; set; }
    public decimal? Quantity { get; set; }
}

public class SetItemStateViewModel
{
    [Required]
    public int ItemId { get; set; }

    [Required]
    public ItemState State { get; set; }
}

public class SetItemCategoryViewModel
{
    [Required]
    public int ItemId { get; set; }

    public int? CategoryId { get; set; }
}

public class SetItemQuantityViewModel
{
    [Required]
    public int ItemId { get; set; }

    [Range(0.01, 999999)]
    public decimal? Quantity { get; set; }
}
