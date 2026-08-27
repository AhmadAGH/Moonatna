using Moonatna.Models;
using Moonatna.Repositories.Families;
using Moonatna.Repositories.Items;

namespace Moonatna.Services.Items
{
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

        public async Task<Item> AddItemAsync(int familyId, string name, int? categoryId, bool isAdHoc, ItemState initialState, int userId)
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
                    CreatedByUserId = userId
                };
                item.Id = await _items.CreateAsync(item);
                return item;
            }

            if (existing.IsArchived)
            {
                // Resurrection: the dead walk again, FKs intact
                await _items.ResurrectAsync(existing.Id, initialState, isAdHoc, userId);
            }
            else if (isAdHoc && existing.State == ItemState.Available)
            {
                // "Add to list" on a live tracked item = mark it out of stock
                await _items.UpdateStateAsync(existing.Id, ItemState.OutOfStock, userId);
            }

            return await _items.GetByIdAsync(existing.Id)
                ?? throw new InvalidOperationException($"Item {existing.Id} vanished mid-operation.");
        }

        public async Task SetStateAsync(int itemId, ItemState state, int userId)
            => await _items.UpdateStateAsync(itemId, state, userId);

        public async Task PurchaseAsync(int itemId, int userId)
        {
            var item = await _items.GetByIdAsync(itemId)
                ?? throw new InvalidOperationException($"Item {itemId} not found.");

            if (!item.IsAdHoc)
            {
                await _items.UpdateStateAsync(itemId, ItemState.Available, userId);
                return;
            }

            var family = await _families.GetByIdAsync(item.FamilyId)
                ?? throw new InvalidOperationException($"Family {item.FamilyId} not found.");

            if (family.AutoPromoteAdHoc)
            {
                await _items.PromoteAsync(itemId, userId);
                return;
            }

            // Delete if free, archive if referenced
            if (await _items.IsReferencedByRecipesAsync(itemId))
                await _items.ArchiveAsync(itemId);
            else
                await _items.DeleteAsync(itemId);
        }

        public async Task SetCategoryAsync(int itemId, int? categoryId)
            => await _items.UpdateCategoryAsync(itemId, categoryId);

        public async Task<Item?> GetByIdAsync(int itemId) => await _items.GetByIdAsync(itemId);
        public async Task<IEnumerable<Item>> GetAllAsync(int familyId) => await _items.GetByFamilyIdAsync(familyId);
        public async Task<bool> SetImageAsync(int familyId, int itemId, string? imagePath)
        {
            var items = await _items.GetByFamilyIdAsync(familyId);
            if (!items.Any(i => i.Id == itemId)) return false; // not this family's item

            await _items.UpdateImagePathAsync(itemId, imagePath);
            return true;
        }
    }
}
