using System.Data;
using Citas.Domain.Entities;
using Citas.Domain.Ports;
using Citas.Infrastructure.Persistence.MySql;
using MySqlConnector;

namespace Citas.Infrastructure.Repositories.MySql;

public class CitaRepositoryMySql : ICitaRepository
{
    private readonly MySqlConnectionFactory _factory;

    public CitaRepositoryMySql(MySqlConnectionFactory factory)
        => _factory = factory;

    public async Task<List<Cita>> ListarAsync()
    {
        var lista = new List<Cita>();

        using var cn = _factory.Create();
        await cn.OpenAsync();

        using var cmd = new MySqlCommand("usp_citas_listar", cn);
        cmd.CommandType = CommandType.StoredProcedure;

        using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
        {
            lista.Add(new Cita
            {
                IdCita = dr.GetInt32("id_cita"),
                Fecha = dr.GetDateTime("fecha"),
                HoraInicio = dr.GetTimeSpan("hora_inicio"),
                Paciente = dr.GetString("paciente"),
                Medico = dr.GetString("medico"),
                Motivo = dr.IsDBNull("motivo") ? "" : dr.GetString("motivo"),
                Precio = dr.GetDecimal("precio"),
                Estado = dr.GetString("estado")
            });
        }

        return lista;
    }

    public async Task CrearAsync(Cita cita)
    {
        using var cn = _factory.Create();
        await cn.OpenAsync();

        using var cmd = new MySqlCommand("usp_citas_crear", cn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@p_id_paciente", cita.IdPaciente);
        cmd.Parameters.AddWithValue("@p_id_medico", cita.IdMedico);
        cmd.Parameters.AddWithValue("@p_fecha", cita.Fecha.Date);
        cmd.Parameters.AddWithValue("@p_hora_inicio", cita.HoraInicio);
        cmd.Parameters.AddWithValue("@p_motivo", cita.Motivo ?? "");
        cmd.Parameters.AddWithValue("@p_precio", cita.Precio);
        cmd.Parameters.AddWithValue("@p_estado", cita.Estado ?? "PENDIENTE");

        await cmd.ExecuteNonQueryAsync();
    }

    // Deja los demás como los tienes o impleméntalos luego:
    public Task<Cita?> BuscarPorIdAsync(int idCita) => Task.FromResult<Cita?>(null);
    public Task ActualizarAsync(Cita cita) => Task.CompletedTask;
    public Task EliminarAsync(int idCita) => Task.CompletedTask;
}
