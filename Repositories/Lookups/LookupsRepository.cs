using Dapper;
using Moonatna.Models;
using Moonatna.Repositories.SqlConnectionFactory;

namespace Moonatna.Repositories.Lookups;

public class LookupsRepository : ILookupsRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public LookupsRepository(ISqlConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
    {
        const string sql = """
            SELECT * FROM [Lookup].[Categories]
            WHERE [IsActive] = 1
            ORDER BY [SortOrder]
            """;

        using var connection = _connectionFactory.Create();
        return await connection.QueryAsync<Category>(sql);
    }

    public async Task<IEnumerable<LocalizationEntry>> GetAllLocalizationsAsync()
    {
        const string sql = "SELECT * FROM [Lookup].[Localizations]";

        using var connection = _connectionFactory.Create();
        return await connection.QueryAsync<LocalizationEntry>(sql);
    }
}