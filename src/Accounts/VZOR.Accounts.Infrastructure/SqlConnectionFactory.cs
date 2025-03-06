using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using VZOR.Core.Database;

namespace VZOR.Accounts.Infrastructure;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly IConfiguration _configuration;
    
    public SqlConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection Create()
        => new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
}