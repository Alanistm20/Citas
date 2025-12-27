namespace Citas.Domain.Entities;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string Username { get; set; } = "";
    public int IdRol { get; set; }
    public string Rol { get; set; } = "";
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }
    public bool Activo { get; set; }
}
