namespace Moonatna.Repositories.Recipes
{
    public class RecipeBadgeCount
    {
        public int RecipeId { get; set; }
        public int MissingCount { get; set; }
        public int RequiredCount { get; set; }
    }
}
