using Microsoft.EntityFrameworkCore; // <-- Los using van aquí arriba
using axcan.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. REGISTRO DE SERVICIOS (Los ingredientes)
builder.Services.AddControllersWithViews();

// Aquí registramos la conexión a Supabase
// Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// -----------------------------------------------------------
var app = builder.Build(); // El Build va solito, sin nada adentro
// -----------------------------------------------------------

// 2. CONFIGURACIÓN DEL PIPELINE (Cómo se cocina)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles(); // Importante para tus CSS y JS
app.UseRouting();
app.UseAuthorization();

// Tus rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=login}/{id?}");

// --- RUTA SECRETA PARA EL CHISME (Déjala por si ocupamos debuguear) ---
app.MapGet("/chisme", () => {
    var root = System.IO.Directory.GetCurrentDirectory();
    var files = System.IO.Directory.GetFiles(root, "*", System.IO.SearchOption.AllDirectories);
    return "ARCHIVOS EN EL SERVIDOR:\n\n" + string.Join("\n", files);
});

app.Run();