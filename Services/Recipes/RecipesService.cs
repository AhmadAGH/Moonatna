using Moonatna.Models;
using Moonatna.Repositories.Items;
using Moonatna.Repositories.Recipes;
using Moonatna.Services.Items;

namespace Moonatna.Services.Recipes
{
    public class RecipesService : IRecipesService
    {
        private readonly IRecipesRepository _recipes;
        private readonly IItemsRepository _items;
        private readonly IItemsService _itemsService;

        public RecipesService(IRecipesRepository recipes, IItemsRepository items, IItemsService itemsService)
            => (_recipes, _items, _itemsService) = (recipes, items, itemsService);

        public async Task<IEnumerable<Recipe>> GetRecipesWithBadgesAsync(int familyId)
            => await _recipes.GetByFamilyIdAsync(familyId);
        // Counts come from GetMissingIngredientCountsAsync — the controller
        // pairs them into the ViewModel; tier labels are presentation, not business.

        public async Task<int> CreateRecipeAsync(Recipe recipe, IEnumerable<RecipeIngredientInput> ingredients, int userId)
        {
            recipe.Id = await _recipes.CreateAsync(recipe);

            foreach (var input in ingredients)
            {
                var itemId = input.ItemId;

                if (itemId is null)
                {
                    // New ingredient: created as OutOfStock so the recipe immediately
                    // shows what's missing. One-off toggle decides IsAdHoc.
                    var item = await _itemsService.AddItemAsync(
                        recipe.FamilyId, input.Name!, null, input.IsAdHoc, ItemState.OutOfStock, userId);
                    itemId = item.Id;
                }

                await _recipes.AddIngredientAsync(new RecipeIngredient
                {
                    RecipeId = recipe.Id,
                    ItemId = itemId.Value,
                    QuantityText = input.QuantityText,
                    IsOptional = input.IsOptional,
                    SortOrder = input.SortOrder
                });
            }

            return recipe.Id;
        }

        public async Task AddMissingToListAsync(int recipeId, int userId)
        {
            var ingredients = await _recipes.GetIngredientsAsync(recipeId);

            foreach (var ingredient in ingredients.Where(i => !i.IsOptional))
            {
                var item = await _items.GetByIdAsync(ingredient.ItemId);
                if (item is null) continue;

                if (item.IsArchived)
                    await _items.ResurrectAsync(item.Id, ItemState.OutOfStock, item.IsAdHoc, userId);
                else if (item.State == ItemState.Available)
                    await _items.UpdateStateAsync(item.Id, ItemState.OutOfStock, userId);
            }
        }

        public async Task<Recipe?> GetByIdAsync(int recipeId) => await _recipes.GetByIdAsync(recipeId);
        public async Task<IEnumerable<RecipeIngredient>> GetIngredientsAsync(int recipeId) => await _recipes.GetIngredientsAsync(recipeId);
        public async Task<Dictionary<int, int>> GetMissingCountsAsync(int familyId) => await _recipes.GetMissingIngredientCountsAsync(familyId);
    }
}
