using Moonatna.Models;

namespace Moonatna.Repositories.Items;

public interface IItemsRepository
{
    Task<IEnumerable<Item>> GetPantryAsync(int familyId);
    Task<IEnumerable<Item>> GetShoppingListAsync(int familyId);
    Task<Item?> GetByIdAsync(int id);
    Task<Item?> GetByNameAsync(int familyId, string name);
    Task<int> CreateAsync(Item item);
    Task UpdateStateAsync(int id, ItemState state, int updatedByUserId);
    Task UpdateQuantityAsync(int id, decimal? quantity, int updatedByUserId);
    Task ResurrectAsync(int id, ItemState state, bool isAdHoc, int updatedByUserId);
    Task SetCategoryAsync(int id, int? categoryId, int updatedByUserId);
    Task ArchiveAsync(int id, int updatedByUserId);
}
