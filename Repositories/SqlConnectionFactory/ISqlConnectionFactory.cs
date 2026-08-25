using System.Data;

namespace Moonatna.Repositories.SqlConnectionFactory
{
    public interface ISqlConnectionFactory
    {
        IDbConnection Create();
    }
}
