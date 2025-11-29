using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data;

public class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    public MySqlConnection CreateConnection()
        => new MySqlConnection(_connectionString);
}
