using Citas.Application.Services;
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
    public async Task<IActionResult> Index()
    {
        var medicos = await _medicoService.ListarAsync();
        return View(medicos);
    }
}
