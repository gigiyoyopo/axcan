using Microsoft.EntityFrameworkCore;
using axcan.Data;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

// 1. REGISTRO DE SERVICIOS
builder.Services.AddControllersWithViews();

// Obtenemos la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registramos el Contexto con Npgsql
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions => {
        // Esto ayuda a que no truene por tiempos de espera en Render
        npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    }));

var app = builder.Build();

// --- EL CHISMOSO (Pon esto para saber si conecta en los Logs de Render) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Intentamos abrir la conexión solo para probar
        context.Database.OpenConnection();
        Console.WriteLine("✅ ¡IGUANO EXITOSO! Conectado a Supabase correctamente.");
        context.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ ERROR DE CONEXIÓN: " + ex.Message);
    }
}
// -----------------------------------------------------------------------

// 2. CONFIGURACIÓN DEL PIPELINE
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=login}/{id?}");

app.Run();