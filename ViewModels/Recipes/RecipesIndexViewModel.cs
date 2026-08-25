namespace Moonatna.ViewModels.Recipes;

public class RecipesIndexViewModel
{
    public int FamilyId { get; set; }
    public List<RecipeCardViewModel> Recipes { get; set; } = new();
}

public class RecipeCardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhotoPath { get; set; }
    public int MissingCount { get; set; }

    public RecipeBadgeTier BadgeTier => MissingCount switch
    {
        0 => RecipeBadgeTier.Doable,
        <= 2 => RecipeBadgeTier.MissingFew,
        _ => RecipeBadgeTier.MissingALot
    };

    public string BadgeKey => $"RecipeBadge.{BadgeTier}";
}

public enum RecipeBadgeTier { Doable, MissingFew, MissingALot }
