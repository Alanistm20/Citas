using System.Data;
using Citas.Domain.Entities;
using Citas.Domain.Ports;
using Citas.Infrastructure.Persistence.MySql;
using MySqlConnector;

namespace Citas.Infrastructure.Repositories.MySql;

public class PacienteRepositoryMySql : IPacienteRepository
{
    private readonly MySqlConnectionFactory _factory;

    public PacienteRepositoryMySql(MySqlConnectionFactory factory) => _factory = factory;

    public async Task<List<Paciente>> ListarAsync()
    {
        var lista = new List<Paciente>();

        using var cn = _factory.Create();
        await cn.OpenAsync();

        using var cmd = new MySqlCommand("usp_pacientes_listar", cn);
        cmd.CommandType = CommandType.StoredProcedure;

        //  IMPORTANTE: si el SP tiene filtro, le pasamos vacío
        cmd.Parameters.AddWithValue("p_filtro", "");

        using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
        {
            lista.Add(new Paciente
            {
                IdPaciente = dr.GetInt32("IdPaciente"),
                Nombre = dr["Nombre"]?.ToString() ?? "",
                Dni = dr["Dni"]?.ToString() ?? "",
                Telefono = dr["Telefono"]?.ToString() ?? "",
                Activo = Convert.ToBoolean(dr["Activo"])
            });
        }

        return lista;
    }
}
