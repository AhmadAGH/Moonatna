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
    public int RequiredCount { get; set; }   // non-optional ingredients only

    public RecipeBadgeTier BadgeTier => MissingCount switch
    {
        0 => RecipeBadgeTier.Doable,
        // Tiny recipes tolerate no gaps; larger ones absorb a few missing items.
        _ when RequiredCount <= 3 => RecipeBadgeTier.MissingALot,
        _ => (double)MissingCount / RequiredCount > 0.45 ? RecipeBadgeTier.MissingALot : RecipeBadgeTier.MissingFew
    };

    public string BadgeKey => $"RecipeBadge.{BadgeTier}";
}

public enum RecipeBadgeTier { Doable, MissingFew, MissingALot }
