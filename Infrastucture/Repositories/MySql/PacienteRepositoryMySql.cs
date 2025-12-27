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


    public async Task CrearAsync(Paciente paciente)
    {
        using var cn = _factory.Create();
        await cn.OpenAsync();

        using var cmd = new MySqlCommand("usp_pacientes_crear", cn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("p_nombre", paciente.Nombre);
        cmd.Parameters.AddWithValue("p_dni", paciente.Dni);
        cmd.Parameters.AddWithValue("p_telefono", paciente.Telefono ?? "");
        cmd.Parameters.AddWithValue("p_activo", paciente.Activo ? 1 : 0);

        await cmd.ExecuteNonQueryAsync();
    }
}
