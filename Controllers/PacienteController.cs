using Citas.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Citas.Infrastructure.Security;

namespace Citas.Controllers;
[RequireLogin]
public class PacientesController : Controller
{
    private readonly PacienteService _pacienteService;

    public PacientesController(PacienteService pacienteService)
        => _pacienteService = pacienteService;

    // LISTAR
    public async Task<IActionResult> Index()
    {
        var pacientes = await _pacienteService.ListarAsync();
        return View(pacientes);
    }
}
