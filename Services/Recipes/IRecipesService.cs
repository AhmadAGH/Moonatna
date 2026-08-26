using Moonatna.Models;
using Moonatna.Repositories.Recipes;

namespace Moonatna.Services.Recipes
{
    public interface IRecipesService
    {
        Task<IEnumerable<Recipe>> GetRecipesWithBadgesAsync(int familyId);
        Task<int> CreateRecipeAsync(Recipe recipe, IEnumerable<RecipeIngredientInput> ingredients, int userId);
        Task UpdateRecipeAsync(Recipe recipe, string name, string? photoPath, IEnumerable<RecipeIngredientInput> ingredients, int userId);
        Task ArchiveAsync(int recipeId);
        Task<bool> RemoveIngredientAsync(int ingredientId, int familyId);
        Task AddMissingToListAsync(int recipeId, int userId);
        Task<Recipe?> GetByIdAsync(int recipeId);
        Task<IEnumerable<RecipeIngredient>> GetIngredientsAsync(int recipeId);
        Task<Dictionary<int, RecipeBadgeCount>> GetIngredientCountsAsync(int familyId);
    }
}
