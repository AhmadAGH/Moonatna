using Moonatna.Models;

namespace Moonatna.Services.Items;

public interface IItemsService
{
    Task<IEnumerable<Item>> GetPantryAsync(int familyId);
    Task<IEnumerable<Item>> GetShoppingListAsync(int familyId);
    Task<Item> AddItemAsync(int familyId, string name, int? categoryId, bool isAdHoc, ItemState initialState, int userId, decimal? quantity = null);
    Task SetStateAsync(int itemId, ItemState state, int userId);
    Task SetQuantityAsync(int itemId, decimal? quantity, int userId);
    Task PurchaseAsync(int itemId, int userId);
    Task SetCategoryAsync(int itemId, int? categoryId);
    Task<Item?> GetByIdAsync(int itemId);
}
