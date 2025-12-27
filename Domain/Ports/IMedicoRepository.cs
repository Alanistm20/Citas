using Citas.Domain.Entities;

namespace Citas.Domain.Ports;

public interface IMedicoRepository
{
    Task<List<Medico>> ListarAsync();
    Task CrearAsync(Medico medico);

}

