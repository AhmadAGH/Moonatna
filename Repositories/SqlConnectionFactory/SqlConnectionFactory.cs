using Microsoft.Data.SqlClient;
using System.Data;

namespace Moonatna.Repositories.SqlConnectionFactory
{
    public class SqlConnectionFactory: ISqlConnectionFactory
    {
        private readonly string _connectionString;
        public SqlConnectionFactory(IConfiguration configuration)
            => _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        public IDbConnection Create() => new SqlConnection(_connectionString);
    }
}
