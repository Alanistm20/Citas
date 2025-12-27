using Citas.Domain.Entities;
using Citas.Domain.Ports;

namespace Citas.Application.Services;

public class MedicoService
{
    private readonly IMedicoRepository _repo;

    public MedicoService(IMedicoRepository repo) => _repo = repo;

    public Task<List<Medico>> ListarAsync() => _repo.ListarAsync();

    public Task CrearAsync(Medico medico) => _repo.CrearAsync(medico);
}
