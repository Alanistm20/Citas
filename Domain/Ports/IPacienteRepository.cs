using Citas.Domain.Entities;

namespace Citas.Domain.Ports;

public interface IPacienteRepository
{
    Task<List<Paciente>> ListarAsync();


    Task CrearAsync(Paciente paciente);
}

