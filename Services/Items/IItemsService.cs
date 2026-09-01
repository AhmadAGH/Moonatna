using Moonatna.Models;

namespace Moonatna.Services.Items
{
    public interface IItemsService
    {
        Task<IEnumerable<Item>> GetPantryAsync(int familyId);
        Task<IEnumerable<Item>> GetShoppingListAsync(int familyId);
        Task<Item> AddItemAsync(int familyId, string name, int? categoryId, bool isAdHoc, ItemState initialState, int userId);
        Task SetStateAsync(int itemId, ItemState state, int userId);
        Task PurchaseAsync(int itemId, int userId);
        Task SetCategoryAsync(int itemId, int? categoryId);
        Task<Item?> GetByIdAsync(int itemId);
        Task<IEnumerable<Item>> GetAllAsync(int familyId);
        Task<bool> SetImageAsync(int familyId, int itemId, string? imagePath);
        Task UpdateItemAsync(int itemId, string name, int? categoryId, int? quantity, int userId);
        Task DeleteItemAsync(int itemId);

    }
}
