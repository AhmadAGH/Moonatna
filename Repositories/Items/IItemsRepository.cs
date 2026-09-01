using Moonatna.Models;

namespace Moonatna.Repositories.Items
{
    public interface IItemsRepository
    {
        Task<IEnumerable<Item>> GetPantryAsync(int familyId);        // IsAdHoc = 0, not archived
        Task<IEnumerable<Item>> GetShoppingListAsync(int familyId);  // State IN (1,2), not archived
        Task<Item?> GetByIdAsync(int id);
        Task<Item?> GetByNameAsync(int familyId, string name);       // INCLUDES archived — resurrection depends on it
        Task<int> CreateAsync(Item item);
        Task UpdateStateAsync(int id, ItemState state, int updatedByUserId);
        Task PromoteAsync(int id, int updatedByUserId);              // IsAdHoc = 0, State = Mojoud
        Task ArchiveAsync(int id);
        Task DeleteAsync(int id);                                    // hard delete — unreferenced ad-hocs only
        Task<bool> IsReferencedByRecipesAsync(int id);
        Task UpdateCategoryAsync(int id, int? categoryId);
        Task UpdateImagePathAsync(int id, string? imagePath);
        Task UpdateAsync(int id, string name, int? categoryId, int? quantity, int updatedByUserId);
        Task ResurrectAsync(int id, ItemState state, bool isAdHoc, int updatedByUserId);
        Task<IEnumerable<Item>> GetByFamilyIdAsync(int familyId);


    }
}
