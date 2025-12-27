namespace Citas.Domain.Entities;

public class Cita
{
    public int IdCita { get; set; }
    public int IdPaciente { get; set; }
    public int IdMedico { get; set; }

    public DateTime Fecha { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin => HoraInicio.Add(TimeSpan.FromMinutes(30));

    public string Motivo { get; set; } = "";
    public decimal Precio { get; set; }
    public string Estado { get; set; } = "";

    // Campos “de vista” (si tu SP devuelve nombres)
    public string? Medico { get; set; }
    public string? Paciente { get; set; }
}
