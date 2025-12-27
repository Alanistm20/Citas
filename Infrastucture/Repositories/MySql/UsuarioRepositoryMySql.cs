using System.Data;
using Citas.Domain.Entities;
using Citas.Domain.Ports;
using Citas.Infrastructure.Persistence.MySql;
using MySqlConnector;

namespace Citas.Infrastructure.Repositories.MySql;

public class UsuarioRepositoryMySql : IUsuarioRepository
{
    private readonly MySqlConnectionFactory _factory;

    public UsuarioRepositoryMySql(MySqlConnectionFactory factory)
        => _factory = factory;

    public async Task<Usuario?> LoginAsync(string username, string password)
    {
        using var cn = _factory.Create();
        await cn.OpenAsync();

        using var cmd = new MySqlCommand("usp_usuarios_login", cn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("p_username", username);
        cmd.Parameters.AddWithValue("p_password", password);

        using var dr = await cmd.ExecuteReaderAsync();
        if (!await dr.ReadAsync()) return null;

        return new Usuario
        {
            IdUsuario = dr.GetInt32("id_usuario"),
            Username = dr.GetString("username"),
            IdRol = dr.GetInt32("id_rol"),
            Rol = dr.GetString("rol"),
            IdPaciente = dr.GetInt32("id_paciente"), 
            IdMedico = dr.GetInt32("id_medico"),
            Activo = dr.GetInt32("activo") == 1
        };
    }
}
