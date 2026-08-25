using Moonatna.Models;

namespace Moonatna.Repositories.Lookups
{
    public interface ILookupsRepository
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        Task<IEnumerable<LocalizationEntry>> GetAllLocalizationsAsync();
    }
}
