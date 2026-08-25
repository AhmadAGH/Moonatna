using Dapper;
using Moonatna.Models;
using Moonatna.Repositories.SqlConnectionFactory;

namespace Moonatna.Repositories.Users;

public class UsersRepository : IUsersRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public UsersRepository(ISqlConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<User?> GetByFirebaseUidAsync(string firebaseUid)
    {
        const string sql = "SELECT * FROM [dbo].[Users] WHERE [FirebaseUid] = @firebaseUid";

        using var connection = _connectionFactory.Create();
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { firebaseUid });
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM [dbo].[Users] WHERE [Id] = @id";

        using var connection = _connectionFactory.Create();
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { id });
    }

    public async Task<int> CreateAsync(User user)
    {
        const string sql = """
            INSERT INTO [dbo].[Users] ([FirebaseUid], [DisplayName])
            VALUES (@FirebaseUid, @DisplayName);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        using var connection = _connectionFactory.Create();
        return await connection.ExecuteScalarAsync<int>(sql, user);
    }
}