using Moonatna.Services.Recipes;

namespace Moonatna.ViewModels.Recipes;

public class RecipeCreateViewModel
{
    public string Name { get; set; } = string.Empty;
    public string? PhotoPath { get; set; }
    public List<RecipeIngredientInput> Ingredients { get; set; } = new();
}
