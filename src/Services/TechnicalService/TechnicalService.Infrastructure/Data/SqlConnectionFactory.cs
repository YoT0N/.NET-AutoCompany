using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace TechnicalService.Infrastructure.Data;

public class SqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public IDbConnection CreateConnection()
        => new MySqlConnection(_connectionString);
}
