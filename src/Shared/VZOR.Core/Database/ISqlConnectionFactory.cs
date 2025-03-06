using System.Data;

namespace VZOR.Core.Database;

public interface ISqlConnectionFactory
{
    IDbConnection Create();
}