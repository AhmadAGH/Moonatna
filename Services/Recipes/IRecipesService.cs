using Moonatna.Models;

namespace Moonatna.Services.Recipes
{
    public interface IRecipesService
    {
        Task<IEnumerable<Recipe>> GetRecipesWithBadgesAsync(int familyId);
        Task<int> CreateRecipeAsync(Recipe recipe, IEnumerable<RecipeIngredientInput> ingredients, int userId);
        Task AddMissingToListAsync(int recipeId, int userId);
        Task<Recipe?> GetByIdAsync(int recipeId);
        Task<IEnumerable<RecipeIngredient>> GetIngredientsAsync(int recipeId);
        Task<Dictionary<int, int>> GetMissingCountsAsync(int familyId);
    }
}
