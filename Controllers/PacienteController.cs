using Citas.Application.Services;
using Citas.Domain.Entities;
using Citas.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;

namespace Citas.Controllers;

[RequireLogin]
public class PacientesController : Controller
{
    private readonly PacienteService _pacienteService;

    public PacientesController(PacienteService pacienteService)
        => _pacienteService = pacienteService;

   
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var pacientes = await _pacienteService.ListarAsync();
        return View(pacientes);
    }

    
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Paciente
        {
            Activo = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Paciente model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _pacienteService.CrearAsync(model);
        return RedirectToAction(nameof(Index));
    }
}
