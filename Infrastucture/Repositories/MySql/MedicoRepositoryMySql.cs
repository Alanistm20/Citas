using System.Data;
using Citas.Domain.Entities;
using Citas.Domain.Ports;
using Citas.Infrastructure.Persistence.MySql;
using MySqlConnector;

namespace Citas.Infrastructure.Repositories.MySql;

public class MedicoRepositoryMySql : IMedicoRepository
{
    private readonly MySqlConnectionFactory _factory;

    public MedicoRepositoryMySql(MySqlConnectionFactory factory) => _factory = factory;

    public async Task<List<Medico>> ListarAsync()
    {
        var lista = new List<Medico>();

        using var cn = _factory.Create();
        await cn.OpenAsync();

        using var cmd = new MySqlCommand("usp_medicos_listar", cn);
        cmd.CommandType = CommandType.StoredProcedure;

        using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
        {
            lista.Add(new Medico
            {
                IdMedico = dr.GetInt32("IdMedico"),
                Nombre = dr["Nombre"]?.ToString() ?? "",
                Especialidad = dr["Especialidad"]?.ToString() ?? "",
                Activo = Convert.ToBoolean(dr["Activo"])
            });
        }

        return lista;
    }

    public async Task CrearAsync(Medico medico)
    {
        using var cn = _factory.Create();
        await cn.OpenAsync();

        using var cmd = new MySqlCommand("usp_medicos_crear", cn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("p_nombre", medico.Nombre);
        cmd.Parameters.AddWithValue("p_especialidad", medico.Especialidad ?? "");
        cmd.Parameters.AddWithValue("p_activo", medico.Activo ? 1 : 0);

        await cmd.ExecuteNonQueryAsync();
    }
}
