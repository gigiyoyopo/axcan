using Microsoft.EntityFrameworkCore;
using axcan.Data;
using Microsoft.AspNetCore.HttpOverrides; // <-- ¡ESTO EVITA QUE SE CAIGA EL SERVER!

var builder = WebApplication.CreateBuilder(args);

// 1. REGISTRO DE SERVICIOS
builder.Services.AddControllersWithViews();

// Conexión a Supabase
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- CONFIGURACIÓN DE GOOGLE ---
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

// 2. CONFIGURACIÓN DEL PIPELINE (EL ORDEN IMPORTA MUCHO AQUÍ)

// Esto arregla el error de "origin_mismatch" en Render
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.Use((context, next) =>
{
    context.Request.Scheme = "https";
    return next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

// El orden sagrado: Authentication siempre antes que Authorization
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=login}/{id?}");

app.Run();