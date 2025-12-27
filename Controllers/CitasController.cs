using Citas.Application.Services;
using Citas.Domain.Entities;
using Citas.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;

namespace Citas.Controllers;

[RequireLogin]
public class CitasController : Controller
{
    private readonly CitaService _citaService;
    private readonly MedicoService _medicoService;
    private readonly PacienteService _pacienteService;

    public CitasController(
        CitaService citaService,
        MedicoService medicoService,
        PacienteService pacienteService)
    {
        _citaService = citaService;
        _medicoService = medicoService;
        _pacienteService = pacienteService;
    }


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var lista = await _citaService.ListarAsync();
        return View(lista);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await CargarCombosAsync();

        var model = new Cita
        {
            Fecha = DateTime.Today,
            HoraInicio = new TimeSpan(9, 0, 0),
            Estado = "PENDIENTE",
            Precio = 0
        };

        return View(model);
    }

    // POST: /Citas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Cita model)
    {
        if (!ModelState.IsValid)
        {
            await CargarCombosAsync();
            return View(model);
        }

        await _citaService.CrearAsync(model);
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var cita = await _citaService.BuscarPorIdAsync(id);
        if (cita == null) return NotFound();

        await CargarCombosAsync();
        return View(cita);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Cita model)
    {
        if (id != model.IdCita) return BadRequest();

        if (!ModelState.IsValid)
        {
            await CargarCombosAsync();
            return View(model);
        }

        await _citaService.ActualizarAsync(model);
        return RedirectToAction(nameof(Index));
    }
   
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var cita = await _citaService.BuscarPorIdAsync(id);
        if (cita == null) return NotFound();

        return View(cita);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _citaService.EliminarAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult HorariosDisponibles(int idMedico, DateTime fecha)
    {
       
        var horarios = new List<string> { "09:00", "09:30", "10:00", "10:30" };
        return Json(horarios);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Reservar([FromBody] ReservarRequest req)
    {
        try
        {
            if (req.IdMedico <= 0) return Json(new { ok = false, mensaje = "Seleccione un médico." });
            if (string.IsNullOrWhiteSpace(req.Fecha)) return Json(new { ok = false, mensaje = "Seleccione una fecha." });
            if (string.IsNullOrWhiteSpace(req.Hora)) return Json(new { ok = false, mensaje = "Seleccione una hora." });

            var idPaciente = HttpContext.Session.GetInt32("id_paciente") ?? 0;
            if (idPaciente == 0) return Json(new { ok = false, mensaje = "No se detectó paciente en sesión." });

            
            return Json(new { ok = true });
        }
        catch (Exception ex)
        {
            return Json(new { ok = false, mensaje = ex.Message });
        }
    }

   
    private async Task CargarCombosAsync()
    {
        ViewBag.Medicos = await _medicoService.ListarAsync();
        ViewBag.Pacientes = await _pacienteService.ListarAsync();
    }
}

public class ReservarRequest
{
    public int IdMedico { get; set; }
    public string Fecha { get; set; } = "";
    public string Hora { get; set; } = "";
    public string Motivo { get; set; } = "";
}
