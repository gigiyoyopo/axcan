using Microsoft.EntityFrameworkCore;
using axcan.Data;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// 1. REGISTRO DE SERVICIOS
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// CONFIGURACIÓN DE GOOGLE (Asegúrate de tener instalada la versión 9.0.0)
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

// 2. CONFIGURACIÓN DEL PIPELINE (ORDEN ESTRICTO PARA EVITAR OVERFLOW)

// Esto es vital: Le dice a la app que confíe plenamente en el Proxy de Render
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// SOLUCIÓN DEFINITIVA AL OVERFLOW Y ORIGIN_MISMATCH:
// Forzamos que la app siempre se reconozca como HTTPS y con el host de Render.
app.Use((context, next) =>
{
    context.Request.Scheme = "https";
    context.Request.Host = new HostString("axcan.onrender.com");
    return next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // ELIMINADO: app.UseHttpsRedirection() -> Esta es la causa principal del bucle infinito en Render.
}

app.UseStaticFiles();
app.UseRouting();

// El orden de estos dos es obligatorio
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=login}/{id?}");

app.Run();