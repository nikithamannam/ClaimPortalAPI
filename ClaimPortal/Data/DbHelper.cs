#pragma warning disable CS0618 // Suppress obsolete warning for System.Data.SqlClient
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class DbHelper
{
    private readonly string _connectionString;

    public DbHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException("Missing connection string");
    }

    public SqlConnection GetConnection() => new SqlConnection(_connectionString);
}
#pragma warning restore CS0618
