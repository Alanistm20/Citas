using Citas.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Citas.Controllers;

public class AuthController : Controller
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
        => _authService = authService;

    // GET: /Auth/Login
    [HttpGet]
    public IActionResult Login()
    {
        // Si ya está logueado, manda a Citas (o Home si existe)
        if (HttpContext.Session.GetInt32("id_usuario") != null)
            return RedirectToAction("Index", "Citas");

        return View();
    }

    // POST: /Auth/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Ingrese usuario y contraseña.";
            return View();
        }

        var user = await _authService.LoginAsync(username.Trim(), password);

        if (user == null)
        {
            ViewBag.Error = "Credenciales inválidas o usuario inactivo.";
            return View();
        }

        // Guardar en sesión
        HttpContext.Session.SetInt32("id_usuario", user.IdUsuario);
        HttpContext.Session.SetString("username", user.Username);
        HttpContext.Session.SetInt32("id_rol", user.IdRol);
        HttpContext.Session.SetString("rol", user.Rol);
        HttpContext.Session.SetInt32("id_paciente", user.IdPaciente);
        HttpContext.Session.SetInt32("id_medico", user.IdMedico);

        // ✅ IMPORTANTE: manda a una ruta que exista
        return RedirectToAction("Index", "Citas");
    }

    // GET: /Auth/Logout
    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    public IActionResult Denied() => View();
}
