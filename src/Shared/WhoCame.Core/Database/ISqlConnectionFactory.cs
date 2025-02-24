using System.Data;

namespace WhoCame.Core.Database;

public interface ISqlConnectionFactory
{
    IDbConnection Create();
}