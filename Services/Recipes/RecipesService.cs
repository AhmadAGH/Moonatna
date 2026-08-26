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

        // Replace-style: keeps the Item links, renames the family's Item when the
        // ingredient text changes. (Field-level sync is a later refinement.)
        public async Task UpdateRecipeAsync(Recipe recipe, string name, string? photoPath,
            IEnumerable<RecipeIngredientInput> ingredients, int userId)
        {
            recipe.Name = name;
            recipe.PhotoPath = photoPath;
            await _recipes.UpdateAsync(recipe);

            var existing = (await _recipes.GetIngredientsAsync(recipe.Id)).ToList();
            foreach (var old in existing)
                await _recipes.DeleteIngredientAsync(old.Id);

            var byId = existing.ToDictionary(e => e.Id);
            foreach (var input in ingredients)
            {
                var itemId = input.ItemId;
                var nameFromRow = input.Name?.Trim();

                if (itemId is null && input.IngredientId is not null && byId.TryGetValue(input.IngredientId.Value, out var prev))
                {
                    itemId = prev.ItemId;
                    if (!string.IsNullOrEmpty(nameFromRow) && nameFromRow.Length > 0)
                    {
                        var item = await _items.GetByIdAsync(itemId.Value);
                        if (item is not null && item.Name != nameFromRow)
                        {
                            item.Name = nameFromRow;
                            item.UpdatedByUserId = userId;
                            await _items.UpdateNameAsync(item);
                        }
                    }
                }
                else if (itemId is null)
                {
                    var match = await _items.GetByNameAsync(recipe.FamilyId, input.Name!.Trim());
                    if (match is not null)
                    {
                        itemId = match.Id;
                        if (match.IsArchived)
                            await _items.ResurrectAsync(match.Id, ItemState.OutOfStock, match.IsAdHoc, userId);
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
        }

        public async Task ArchiveAsync(int recipeId) => await _recipes.ArchiveAsync(recipeId);

        public async Task<bool> RemoveIngredientAsync(int ingredientId, int familyId)
        {
            var ingredient = await _recipes.GetIngredientByIdAsync(ingredientId);
            if (ingredient is null) return false;

            var recipe = await _recipes.GetByIdAsync(ingredient.RecipeId);
            if (recipe is null || recipe.FamilyId != family.Id) return false;

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
