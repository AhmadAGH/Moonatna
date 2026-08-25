using Dapper;
using Moonatna.Models;
using Moonatna.Repositories.SqlConnectionFactory;

namespace Moonatna.Repositories.Families;

public class FamiliesRepository : IFamiliesRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public FamiliesRepository(ISqlConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<Family?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM [dbo].[Families] WHERE [Id] = @id";

        using var connection = _connectionFactory.Create();
        return await connection.QuerySingleOrDefaultAsync<Family>(sql, new { id });
    }

    public async Task<Family?> GetByJoinCodeAsync(string joinCode)
    {
        const string sql = "SELECT * FROM [dbo].[Families] WHERE [JoinCode] = @joinCode";

        using var connection = _connectionFactory.Create();
        return await connection.QuerySingleOrDefaultAsync<Family>(sql, new { joinCode });
    }

    public async Task<int> CreateAsync(Family family)
    {
        const string sql = """
            INSERT INTO [dbo].[Families] ([Name], [JoinCode], [AutoPromoteAdHoc], [CreatedByUserId])
            VALUES (@Name, @JoinCode, @AutoPromoteAdHoc, @CreatedByUserId);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        using var connection = _connectionFactory.Create();
        return await connection.ExecuteScalarAsync<int>(sql, family);
    }

    public async Task UpdateAsync(Family family)
    {
        const string sql = """
            UPDATE [dbo].[Families]
            SET [Name] = @Name, [AutoPromoteAdHoc] = @AutoPromoteAdHoc
            WHERE [Id] = @Id
            """;

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, family);
    }

    // ============ Members ============

    public async Task<FamilyMember?> GetMembershipAsync(int familyId, int userId)
    {
        const string sql = """
            SELECT * FROM [dbo].[FamilyMembers]
            WHERE [FamilyId] = @familyId AND [UserId] = @userId
            """;

        using var connection = _connectionFactory.Create();
        return await connection.QuerySingleOrDefaultAsync<FamilyMember>(sql, new { familyId, userId });
    }

    public async Task<IEnumerable<Family>> GetFamiliesByUserIdAsync(int userId)
    {
        const string sql = """
            SELECT f.* FROM [dbo].[Families] f
            INNER JOIN [dbo].[FamilyMembers] fm ON fm.[FamilyId] = f.[Id]
            WHERE fm.[UserId] = @userId
            ORDER BY f.[Name]
            """;

        using var connection = _connectionFactory.Create();
        return await connection.QueryAsync<Family>(sql, new { userId });
    }

    public async Task AddMemberAsync(FamilyMember member)
    {
        const string sql = """
            INSERT INTO [dbo].[FamilyMembers] ([FamilyId], [UserId], [Role])
            VALUES (@FamilyId, @UserId, @Role);
            """;

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, member);
    }

    public async Task RemoveMemberAsync(int familyId, int userId)
    {
        const string sql = """
            DELETE FROM [dbo].[FamilyMembers]
            WHERE [FamilyId] = @familyId AND [UserId] = @userId
            """;

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, new { familyId, userId });
    }

    public async Task<IEnumerable<FamilyMemberInfo>> GetMembersAsync(int familyId)
    {
        const string sql = """
        SELECT fm.[UserId], u.[DisplayName], fm.[Role]
        FROM [dbo].[FamilyMembers] fm
        INNER JOIN [dbo].[Users] u ON u.[Id] = fm.[UserId]
        WHERE fm.[FamilyId] = @familyId
        ORDER BY fm.[Role] DESC, u.[DisplayName]
        """;

        using var connection = _connectionFactory.Create();
        return await connection.QueryAsync<FamilyMemberInfo>(sql, new { familyId });
    }

    public async Task UpdateAutoPromoteAsync(int id, bool autoPromoteAdHoc)
    {
        const string sql = "UPDATE [dbo].[Families] SET [AutoPromoteAdHoc] = @autoPromoteAdHoc WHERE [Id] = @id";

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, new { id, autoPromoteAdHoc });
    }
}