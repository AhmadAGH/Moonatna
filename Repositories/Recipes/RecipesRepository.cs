using Dapper;
using Moonatna.Models;
using Moonatna.Repositories.SqlConnectionFactory;

namespace Moonatna.Repositories.Recipes;

public class RecipesRepository : IRecipesRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public RecipesRepository(ISqlConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<IEnumerable<Recipe>> GetByFamilyIdAsync(int familyId)
    {
        const string sql = """
            SELECT * FROM [dbo].[Recipes]
            WHERE [FamilyId] = @familyId AND [IsArchived] = 0
            ORDER BY [Name]
            """;

        using var connection = _connectionFactory.Create();
        return await connection.QueryAsync<Recipe>(sql, new { familyId });
    }

    public async Task<Recipe?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM [dbo].[Recipes] WHERE [Id] = @id AND [IsArchived] = 0";

        using var connection = _connectionFactory.Create();
        return await connection.QuerySingleOrDefaultAsync<Recipe>(sql, new { id });
    }

    public async Task<int> CreateAsync(Recipe recipe)
    {
        const string sql = """
            INSERT INTO [dbo].[Recipes] ([FamilyId], [Name], [Steps], [PhotoPath], [CreatedByUserId])
            VALUES (@FamilyId, @Name, @Steps, @PhotoPath, @CreatedByUserId);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        using var connection = _connectionFactory.Create();
        return await connection.ExecuteScalarAsync<int>(sql, recipe);
    }

    public async Task UpdateAsync(Recipe recipe)
    {
        const string sql = """
            UPDATE [dbo].[Recipes]
            SET [Name] = @Name, [Steps] = @Steps, [PhotoPath] = @PhotoPath
            WHERE [Id] = @Id
            """;

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, recipe);
    }

    public async Task ArchiveAsync(int id)
    {
        const string sql = "UPDATE [dbo].[Recipes] SET [IsArchived] = 1 WHERE [Id] = @id";

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, new { id });
    }

    // ============ Ingredients ============

    public async Task<IEnumerable<RecipeIngredient>> GetIngredientsAsync(int recipeId)
    {
        const string sql = """
            SELECT * FROM [dbo].[RecipeIngredients]
            WHERE [RecipeId] = @recipeId
            ORDER BY [SortOrder], [Id]
            """;

        using var connection = _connectionFactory.Create();
        return await connection.QueryAsync<RecipeIngredient>(sql, new { recipeId });
    }

    public async Task AddIngredientAsync(RecipeIngredient ingredient)
    {
        const string sql = """
            INSERT INTO [dbo].[RecipeIngredients] ([RecipeId], [ItemId], [QuantityText], [IsOptional], [SortOrder])
            VALUES (@RecipeId, @ItemId, @QuantityText, @IsOptional, @SortOrder);
            """;

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, ingredient);
    }

    public async Task DeleteIngredientAsync(int id)
    {
        const string sql = "DELETE FROM [dbo].[RecipeIngredients] WHERE [Id] = @id";

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, new { id });
    }

    public async Task<Dictionary<int, int>> GetMissingIngredientCountsAsync(int familyId)
    {
        const string sql = """
        SELECT r.[Id] AS [RecipeId],
               COUNT(CASE WHEN ri.[IsOptional] = 0 AND (i.[State] <> 0 OR i.[IsArchived] = 1) THEN 1 END) AS [MissingCount]
        FROM [dbo].[Recipes] r
        LEFT JOIN [dbo].[RecipeIngredients] ri ON ri.[RecipeId] = r.[Id]
        LEFT JOIN [dbo].[Items] i ON i.[Id] = ri.[ItemId]
        WHERE r.[FamilyId] = @familyId AND r.[IsArchived] = 0
        GROUP BY r.[Id]
        """;

        using var connection = _connectionFactory.Create();
        var rows = await connection.QueryAsync<RecipeBadgeCount>(sql, new { familyId });
        return rows.ToDictionary(x => x.RecipeId, x => x.MissingCount);
    }
}