using Citas.Domain.Entities;
using Citas.Domain.Ports;

namespace Citas.Application.Services;

public class CitaService
{
    private readonly ICitaRepository _repo;

    public CitaService(ICitaRepository repo)
        => _repo = repo;

    public Task<List<Cita>> ListarAsync() => _repo.ListarAsync();

    public Task<Cita?> BuscarPorIdAsync(int idCita) => _repo.BuscarPorIdAsync(idCita);

    public Task CrearAsync(Cita cita) => _repo.CrearAsync(cita);

    public Task ActualizarAsync(Cita cita) => _repo.ActualizarAsync(cita);

    public Task EliminarAsync(int idCita) => _repo.EliminarAsync(idCita);
}
