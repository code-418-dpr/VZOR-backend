using System.Data;

namespace WhoCame.Accounts.Application.Database;

public interface ISqlConnectionFactory
{
    IDbConnection Create();
}