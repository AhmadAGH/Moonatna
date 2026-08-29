using Moonatna.Models;
using Moonatna.Repositories.Families;
using Moonatna.Repositories.Items;

namespace Moonatna.Services.Items;

public class ItemsService : IItemsService
{
    private readonly IItemsRepository _items;
    private readonly IFamiliesRepository _families;

    public ItemsService(IItemsRepository items, IFamiliesRepository families)
        => (_items, _families) = (items, families);

    public async Task<IEnumerable<Item>> GetPantryAsync(int familyId)
        => await _items.GetPantryAsync(familyId);

    public async Task<IEnumerable<Item>> GetShoppingListAsync(int familyId)
        => await _items.GetShoppingListAsync(familyId);

    public async Task<Item> AddItemAsync(int familyId, string name, int? categoryId, bool isAdHoc, ItemState initialState, int userId, decimal? quantity = null)
    {
        var normalized = name.Trim();
        var existing = await _items.GetByNameAsync(familyId, normalized);

        if (existing is null)
        {
            var item = new Item
            {
                FamilyId = familyId,
                Name = normalized,
                CategoryId = categoryId,
                State = initialState,
                IsAdHoc = isAdHoc,
                CreatedByUserId = userId,
                Quantity = quantity
            };
            item.Id = await _items.CreateAsync(item);
            return item;
        }

        if (existing.IsArchived)
        {
            await _items.ResurrectAsync(existing.Id, initialState, isAdHoc, userId);
            if (quantity.HasValue)
            {
                await _items.UpdateQuantityAsync(existing.Id, quantity, userId);
            }
        }
        else if (isAdHoc && existing.State == ItemState.Available)
        {
            await _items.UpdateStateAsync(existing.Id, ItemState.OutOfStock, userId);
        }

        return await _items.GetByIdAsync(existing.Id)
            ?? throw new InvalidOperationException($"Item {existing.Id} vanished mid-operation.");
    }

    public async Task SetStateAsync(int itemId, ItemState state, int userId)
        => await _items.UpdateStateAsync(itemId, state, userId);

    public async Task SetQuantityAsync(int itemId, decimal? quantity, int userId)
        => await _items.UpdateQuantityAsync(itemId, quantity, userId);

    public async Task PurchaseAsync(int itemId, int userId)
        => await _items.UpdateStateAsync(itemId, ItemState.Available, userId);

    public async Task SetCategoryAsync(int itemId, int? categoryId)
        => await _items.SetCategoryAsync(itemId, categoryId, 0);

    public async Task<Item?> GetByIdAsync(int itemId)
        => await _items.GetByIdAsync(itemId);
}
