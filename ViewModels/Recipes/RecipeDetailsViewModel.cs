namespace Moonatna.ViewModels.Recipes;

public class RecipeDetailsViewModel
{
    public int Id { get; set; }
    public int FamilyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhotoPath { get; set; }
    public int MissingCount { get; set; }
    public List<RecipeIngredientViewModel> Ingredients { get; set; } = new();
}

public class RecipeIngredientViewModel
{
    public int ItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? QuantityText { get; set; }
    public bool IsOptional { get; set; }
    public bool IsAvailable { get; set; }   // drives the missing/not styling
}
