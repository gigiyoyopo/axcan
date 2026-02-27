using Microsoft.EntityFrameworkCore;
using axcan.Data;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// 1. SERVICIOS
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// CONFIGURACIÓN DE GOOGLE
builder.Services.AddAuthentication(options => {
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Google";
})
.AddCookie("Cookies")
.AddGoogle("Google", options => {
    // Estas líneas lanzan los warnings de tus logs, pero funcionan
    options.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? "";
    options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") ?? "";
});

var app = builder.Build();

// 2. CONFIGURACIÓN DEL PIPELINE (FORZADO PARA RENDER)

// Esto fuerza a la app a ignorar que internamente es HTTP y presentarse como HTTPS ante Google
app.Use((context, next) =>
{
    context.Request.Scheme = "https";
    context.Request.Host = new HostString("axcan.onrender.com");
    return next();
});

// Indispensable para que Google reciba la IP correcta
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=login}/{id?}");

app.Run();