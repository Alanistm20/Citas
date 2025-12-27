using Citas.Domain.Entities;
using Citas.Domain.Ports;

namespace Citas.Application.Services;

public class PacienteService
{
    private readonly IPacienteRepository _repo;

    public PacienteService(IPacienteRepository repo) => _repo = repo;

    public Task<List<Paciente>> ListarAsync() => _repo.ListarAsync();
}

