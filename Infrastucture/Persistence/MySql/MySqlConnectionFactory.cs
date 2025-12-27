using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Citas.Infrastructure.Persistence.MySql;

public class MySqlConnectionFactory
{
    private readonly IConfiguration _config;

    public MySqlConnectionFactory(IConfiguration config)
        => _config = config;

    public MySqlConnection Create()
        => new MySqlConnection(_config.GetConnectionString("cadena"));
}
