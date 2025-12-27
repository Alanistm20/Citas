using Citas.Application.Services;
using Citas.Domain.Ports;
using Citas.Infrastructure.Persistence.MySql;
using Citas.Infrastructure.Repositories.MySql;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<MySqlConnectionFactory>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositoryMySql>();
builder.Services.AddScoped<ICitaRepository, CitaRepositoryMySql>();
builder.Services.AddScoped<IMedicoRepository, MedicoRepositoryMySql>();
builder.Services.AddScoped<IPacienteRepository, PacienteRepositoryMySql>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CitaService>();
builder.Services.AddScoped<MedicoService>();
builder.Services.AddScoped<PacienteService>();
var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();   
app.UseAuthorization(); 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");
app.Run();

