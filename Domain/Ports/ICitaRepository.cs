using Citas.Domain.Entities;

namespace Citas.Domain.Ports;

public interface ICitaRepository
{
    Task<List<Cita>> ListarAsync();
    Task<Cita?> BuscarPorIdAsync(int idCita);
    Task CrearAsync(Cita cita);
    Task ActualizarAsync(Cita cita);
    Task EliminarAsync(int idCita);
}
