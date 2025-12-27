using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Citas.Infrastructure.Security;

public class RequireRoleAttribute : ActionFilterAttribute
{
    private readonly string[] _roles;

    public RequireRoleAttribute(params string[] roles)
    {
        _roles = roles.Select(r => r.ToUpperInvariant()).ToArray();
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var idUsuario = session.GetInt32("id_usuario");
        var rol = (session.GetString("rol") ?? "").ToUpperInvariant();

        if (idUsuario == null)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        if (!_roles.Contains(rol))
        {
            context.Result = new RedirectToActionResult("Denied", "Auth", null);
            return;
        }

        base.OnActionExecuting(context);
    }
}
