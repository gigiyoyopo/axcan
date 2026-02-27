using Microsoft.EntityFrameworkCore;
using axcan.Data;
using Microsoft.AspNetCore.HttpOverrides; // <-- Indispensable para que funcione el proxy de Render

var builder = WebApplication.CreateBuilder(args);

// 1. REGISTRO DE SERVICIOS
builder.Services.AddControllersWithViews();

// Conexión a Supabase
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- CONFIGURACIÓN DE GOOGLE ---
// Usamos variables de entorno para que Render las lea de forma segura
builder.Services.AddAuthentication(options => {
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Google";
})
.AddCookie("Cookies")
.AddGoogle("Google", options => {
    options.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
    options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
});

var app = builder.Build();

// 2. CONFIGURACIÓN DEL PIPELINE (ORDEN CRÍTICO)

// Configuración para que la app entienda que está detrás de un proxy (Render)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// ESTO ROMPE EL BUCLE DE REDIRECCIÓN (OVERFLOW):
// En lugar de forzar "https" a ciegas, leemos lo que el proxy de Render nos dice.
app.Use((context, next) =>
{
    if (context.Request.Headers.TryGetValue("X-Forwarded-Proto", out var proto))
    {
        context.Request.Scheme = proto;
    }
    return next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // NO habilites UseHttpsRedirection aquí, Render ya maneja el SSL por ti.
}

app.UseStaticFiles();
app.UseRouting();

// Orden sagrado: Autenticación antes que Autorización
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=login}/{id?}");

app.Run();