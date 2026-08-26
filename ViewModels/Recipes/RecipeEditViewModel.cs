using Moonatna.Services.Recipes;

namespace Moonatna.ViewModels.Recipes;

public class RecipeEditViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhotoPath { get; set; }
    public List<RecipeEditIngredientViewModel> Ingredients { get; set; } = new();
}

public class RecipeEditIngredientViewModel
{
    public int Id { get; set; }          // RecipeIngredient.Id — 0 for new rows
    public int ItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? QuantityText { get; set; }
    public bool IsOptional { get; set; }
    public int SortOrder { get; set; }
    public bool IsAdHoc { get; set; }
}

public class RemoveIngredientViewModel
{
    public int RecipeId { get; set; }
    public int IngredientId { get; set; }
}
