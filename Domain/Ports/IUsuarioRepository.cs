using Citas.Domain.Entities;

namespace Citas.Domain.Ports;

public interface IUsuarioRepository
{
    Task<Usuario?> LoginAsync(string username, string password);
}
