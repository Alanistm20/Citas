namespace Citas.Domain.Entities;

public class Paciente
{
    public int IdPaciente { get; set; }
    public string Nombre { get; set; } = "";
    public string Dni { get; set; } = "";
    public string Telefono { get; set; } = "";
    public bool Activo { get; set; }
}
