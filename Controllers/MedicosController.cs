using Citas.Application.Services;
using Citas.Domain.Entities;
using Citas.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;

namespace Citas.Controllers;

[RequireLogin]
public class MedicosController : Controller
{
    private readonly MedicoService _medicoService;

    public MedicosController(MedicoService medicoService)
        => _medicoService = medicoService;

    // LISTAR
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var medicos = await _medicoService.ListarAsync();
        return View(medicos);
    }

    // CREATE (GET)
    [HttpGet]
    public IActionResult Create()
    {
        var model = new Medico { Activo = true };
        return View(model);
    }

    // CREATE (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Medico model)
    {
        if (!ModelState.IsValid) return View(model);

        await _medicoService.CrearAsync(model); 
        return RedirectToAction(nameof(Index));
    }
}
