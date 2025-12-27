namespace Citas.Domain.Entities;

public class Medico
{
    public int IdMedico { get; set; }
    public string Nombre { get; set; } = "";
    public string Especialidad { get; set; } = "";
    public bool Activo { get; set; }
}

