using Dapper;
using Moonatna.Models;
using Moonatna.Repositories.SqlConnectionFactory;

namespace Moonatna.Repositories.Items;

public class ItemsRepository : IItemsRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ItemsRepository(ISqlConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IEnumerable<Item>> GetPantryAsync(int familyId)
    {
        const string sql = """
            SELECT * FROM [dbo].[Items]
            WHERE [FamilyId] = @familyId AND [IsAdHoc] = 0 AND [IsArchived] = 0
            ORDER BY [Name]
            """;

        using var connection = _connectionFactory.Create();
        return await connection.QueryAsync<Item>(sql, new { familyId });
    }

    public async Task<IEnumerable<Item>> GetShoppingListAsync(int familyId)
    {
        const string sql = """
            SELECT * FROM [dbo].[Items]
            WHERE [FamilyId] = @familyId AND [State] IN (1, 2) AND [IsArchived] = 0
            ORDER BY [State] DESC, [Name]
            """;

        using var connection = _connectionFactory.Create();
        return await connection.QueryAsync<Item>(sql, new { familyId });
    }

    public async Task<Item?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM [dbo].[Items] WHERE [Id] = @id";

        using var connection = _connectionFactory.Create();
        return await connection.QuerySingleOrDefaultAsync<Item>(sql, new { id });
    }

    public async Task<Item?> GetByNameAsync(int familyId, string name)
    {
        // Deliberately no IsArchived filter — the resurrection rule needs to find the dead.
        const string sql = "SELECT * FROM [dbo].[Items] WHERE [FamilyId] = @familyId AND [Name] = @name";

        using var connection = _connectionFactory.Create();
        return await connection.QuerySingleOrDefaultAsync<Item>(sql, new { familyId, name });
    }

    public async Task<int> CreateAsync(Item item)
    {
        const string sql = """
            INSERT INTO [dbo].[Items] ([FamilyId], [Name], [CategoryId], [State], [IsAdHoc], [ImagePath], [CreatedByUserId])
            VALUES (@FamilyId, @Name, @CategoryId, @State, @IsAdHoc, @ImagePath, @CreatedByUserId);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        using var connection = _connectionFactory.Create();
        return await connection.ExecuteScalarAsync<int>(sql, item);
    }

    public async Task UpdateStateAsync(int id, ItemState state, int updatedByUserId)
    {
        const string sql = """
            UPDATE [dbo].[Items]
            SET [State] = @state, [UpdatedByUserId] = @updatedByUserId, [UpdatedAt] = GETDATE()
            WHERE [Id] = @id
            """;

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, new { id, state, updatedByUserId });
    }

    public async Task PromoteAsync(int id, int updatedByUserId)
    {
        const string sql = """
            UPDATE [dbo].[Items]
            SET [IsAdHoc] = 0, [State] = 0, [UpdatedByUserId] = @updatedByUserId, [UpdatedAt] = GETDATE()
            WHERE [Id] = @id
            """;

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, new { id, updatedByUserId });
    }

    public async Task ArchiveAsync(int id)
    {
        const string sql = "UPDATE [dbo].[Items] SET [IsArchived] = 1 WHERE [Id] = @id";

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, new { id });
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = "DELETE FROM [dbo].[Items] WHERE [Id] = @id";

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, new { id });
    }

    public async Task<bool> IsReferencedByRecipesAsync(int id)
    {
        const string sql = "SELECT COUNT(1) FROM [dbo].[RecipeIngredients] WHERE [ItemId] = @id";

        using var connection = _connectionFactory.Create();
        var count = await connection.ExecuteScalarAsync<int>(sql, new { id });
        return count > 0;
    }

    public async Task UpdateCategoryAsync(int id, int? categoryId)
    {
        const string sql = "UPDATE [dbo].[Items] SET [CategoryId] = @categoryId WHERE [Id] = @id";

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, new { id, categoryId });
    }

    public async Task UpdateImagePathAsync(int id, string? imagePath)
    {
        const string sql = "UPDATE [dbo].[Items] SET [ImagePath] = @imagePath WHERE [Id] = @id";

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, new { id, imagePath });
    }

    public async Task ResurrectAsync(int id, ItemState state, bool isAdHoc, int updatedByUserId)
    {
        const string sql = """
        UPDATE [dbo].[Items]
        SET [IsArchived] = 0, [State] = @state, [IsAdHoc] = @isAdHoc,
            [UpdatedByUserId] = @updatedByUserId, [UpdatedAt] = GETDATE()
        WHERE [Id] = @id
        """;

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, new { id, state, isAdHoc, updatedByUserId });
    }

    public async Task<IEnumerable<Item>> GetByFamilyIdAsync(int familyId)
    {
        const string sql = """
        SELECT [Id], [FamilyId], [CategoryId], [Name], [State], [IsAdHoc], [IsArchived],
               [CreatedByUserId], [CreatedAt], [UpdatedByUserId], [UpdatedAt]
        FROM [dbo].[Items]
        WHERE [FamilyId] = @familyId AND [IsArchived] = 0
        ORDER BY [Name]
        """;

        using var connection = _connectionFactory.Create();
        return await connection.QueryAsync<Item>(sql, new { familyId });
    }
}