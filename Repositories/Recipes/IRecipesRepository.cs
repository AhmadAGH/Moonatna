using Moonatna.Models;

namespace Moonatna.Repositories.Recipes
{
    public interface IRecipesRepository
    {
        Task<IEnumerable<Recipe>> GetByFamilyIdAsync(int familyId);
        Task<Recipe?> GetByIdAsync(int id);
        Task<int> CreateAsync(Recipe recipe);
        Task UpdateAsync(Recipe recipe);
        Task ArchiveAsync(int id);

        // Ingredients
        Task<IEnumerable<RecipeIngredient>> GetIngredientsAsync(int recipeId);
        Task AddIngredientAsync(RecipeIngredient ingredient);
        Task DeleteIngredientAsync(int id);
        Task<Dictionary<int, int>> GetMissingIngredientCountsAsync(int familyId);

    }
}
