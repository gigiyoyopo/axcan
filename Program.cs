using Microsoft.EntityFrameworkCore;
using axcan.Data;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// 1. REGISTRO DE SERVICIOS
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// CONFIGURACIÓN DE GOOGLE (v9.0.0)
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

// 2. CONFIGURACIÓN DEL PIPELINE (ORDEN CRÍTICO PARA RENDER)

// A) Quitamos el ForwardedHeaders automático que a veces falla en Render
// B) FORZAMOS la identidad externa. Con esto, la app deja de "adivinar" y de redireccionar.
app.Use((context, next) =>
{
    context.Request.Scheme = "https";
    context.Request.Host = new HostString("axcan.onrender.com");
    return next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // PROHIBIDO: app.UseHttpsRedirection() -> Esto es lo que causa el Overflow.
    // PROHIBIDO: app.UseHsts() -> También puede causar bucles en proxies.
}

app.UseStaticFiles();
app.UseRouting();

// El orden de estos dos es sagrado
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=login}/{id?}");

app.Run();