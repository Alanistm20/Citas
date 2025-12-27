using Citas.Domain.Entities;
using Citas.Domain.Ports;

namespace Citas.Application.Services;

public class AuthService
{
    private readonly IUsuarioRepository _repo;

    public AuthService(IUsuarioRepository repo)
        => _repo = repo;

    public Task<Usuario?> LoginAsync(string username, string password)
        => _repo.LoginAsync(username, password);
}
