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
        // Counts come from GetIngredientCountsAsync — the controller
        // pairs them into the ViewModel; tier labels are presentation, not business.

        public async Task<int> CreateRecipeAsync(Recipe recipe, IEnumerable<RecipeIngredientInput> ingredients, int userId)
        {
            recipe.Id = await _recipes.CreateAsync(recipe);

            foreach (var input in ingredients)
            {
                var itemId = input.ItemId;

                if (itemId is null)
                {
                    var existing = await _items.GetByNameAsync(recipe.FamilyId, input.Name!.Trim());
                    if (existing is not null)
                    {
                        itemId = existing.Id;
                        if (existing.IsArchived)
                            await _items.ResurrectAsync(existing.Id, ItemState.OutOfStock, existing.IsAdHoc, userId);
                    }
                    else
                    {
                        var item = await _itemsService.AddItemAsync(
                            recipe.FamilyId, input.Name!.Trim(), null, input.IsAdHoc, ItemState.OutOfStock, userId);
                        itemId = item.Id;
                    }
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

        // Replace-style: rows are re-inserted from the builder payload. A row whose
        // name is unchanged keeps its Item link; a renamed row is re-matched by name
        // (linking to an existing pantry item) or creates a new one.
        public async Task UpdateRecipeAsync(Recipe recipe, string name, string? photoPath,
    IEnumerable<RecipeIngredientInput> ingredients, int userId)
        {
            recipe.Name = name;
            recipe.PhotoPath = photoPath;
            await _recipes.UpdateAsync(recipe);

            var existing = await _recipes.GetIngredientsAsync(recipe.Id);
            foreach (var old in existing)
                await _recipes.DeleteIngredientAsync(old.Id);

            foreach (var input in ingredients)
            {
                var rowName = input.Name?.Trim();
                if (string.IsNullOrEmpty(rowName)) continue;

                var itemId = input.ItemId;

                if (itemId is not null)
                {
                    var current = await _items.GetByIdAsync(itemId.Value);
                    if (current is null || current.Name != rowName)
                        itemId = null; // renamed or stale link — re-match by name below
                }

                if (itemId is null)
                {
                    var match = await _items.GetByNameAsync(recipe.FamilyId, rowName);
                    if (match is not null)
                    {
                        itemId = match.Id;
                        if (match.IsArchived)
                            await _items.ResurrectAsync(match.Id, ItemState.OutOfStock, match.IsAdHoc, userId);
                    }
                    else
                    {
                        var item = await _itemsService.AddItemAsync(
                            recipe.FamilyId, rowName, null, input.IsAdHoc, ItemState.OutOfStock, userId);
                        itemId = item.Id;
                    }
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
        }

        public async Task ArchiveAsync(int recipeId) => await _recipes.ArchiveAsync(recipeId);

        public async Task<bool> RemoveIngredientAsync(int recipeId, int ingredientId, int familyId)
        {
            var recipe = await _recipes.GetByIdAsync(recipeId);
            if (recipe is null || recipe.FamilyId != familyId) return false;

            var ingredients = await _recipes.GetIngredientsAsync(recipeId);
            if (!ingredients.Any(i => i.Id == ingredientId)) return false;

            await _recipes.DeleteIngredientAsync(ingredientId);
            return true;
        }

        public async Task AddMissingToListAsync(int recipeId, int userId)
        {
            var ingredients = await _recipes.GetIngredientsAsync(recipeId);

            foreach (var ing in ingredients.Where(i => !i.IsOptional))
            {
                var item = await _items.GetByIdAsync(ing.ItemId);
                if (item is null) continue;

                if (item.IsArchived)
                    await _items.ResurrectAsync(item.Id, ItemState.OutOfStock, item.IsAdHoc, userId);
                else if (item.State == ItemState.Available)
                    await _items.UpdateStateAsync(item.Id, ItemState.OutOfStock, userId);
            }
        }

        public async Task<Recipe?> GetByIdAsync(int recipeId) => await _recipes.GetByIdAsync(recipeId);
        public async Task<IEnumerable<RecipeIngredient>> GetIngredientsAsync(int recipeId) => await _recipes.GetIngredientsAsync(recipeId);
        public async Task<Dictionary<int, RecipeBadgeCount>> GetIngredientCountsAsync(int familyId) => await _recipes.GetIngredientCountsAsync(familyId);
    }
}
