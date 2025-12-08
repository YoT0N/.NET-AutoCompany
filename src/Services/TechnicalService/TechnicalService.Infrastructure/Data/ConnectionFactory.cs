using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace TechnicalService.Dal.Data;

public class SqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public MySqlConnection CreateConnection()
        => new MySqlConnection(_connectionString);
}
