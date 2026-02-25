using Microsoft.EntityFrameworkCore;
using axcan.Data;
using Microsoft.AspNetCore.HttpOverrides; // <-- Necesario para Render/Docker

var builder = WebApplication.CreateBuilder(args);

// 1. REGISTRO DE SERVICIOS
builder.Services.AddControllersWithViews();

// Conexión dinámica: Si existe la variable en Render la usa, si no, usa la local
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                      ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- AQUÍ DEBES AGREGAR EL SERVICIO DE GOOGLE ---
// Si no lo tienes aquí, el botón de Google nunca va a "hablar" con el backend
builder.Services.AddAuthentication(options => {
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Google";
})
.AddCookie("Cookies")
.AddGoogle("Google", options => {
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
});

var app = builder.Build();

// 2. CONFIGURACIÓN DEL PIPELINE

// --- CRÍTICO PARA DOCKER EN RENDER ---
// Esto arregla el error de "Scheme" (http vs https)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Forzamos HTTPS para que Google no se queje
app.Use((context, next) =>
{
    context.Request.Scheme = "https";
    return next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // Asegura que nadie entre por http
app.UseStaticFiles();
app.UseRouting();

// EL ORDEN IMPORTANTE: Auth va antes que Authorization
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=login}/{id?}");

app.Run();