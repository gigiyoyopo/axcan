using Microsoft.EntityFrameworkCore;
using axcan.Data;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

// --- 1. REGISTRO DE SERVICIOS (TODO lo que sea builder.Services va AQUÍ) ---

builder.Services.AddControllersWithViews();

// Obtenemos la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registramos el Contexto con Npgsql
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions => {
        npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    }));

// CONFIGURACIÓN DE SESIÓN (Movido arriba de builder.Build)
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// --- LA LÍNEA SAGRADA ---
var app = builder.Build();

// --- EL CHISMOSO (Logs de Render) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.OpenConnection();
        Console.WriteLine("✅ ¡IGUANO EXITOSO! Conectado a Supabase correctamente.");
        context.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ ERROR DE CONEXIÓN: " + ex.Message);
    }
}

// --- 2. CONFIGURACIÓN DEL PIPELINE (TODO lo que sea app.Use va AQUÍ) ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

// ACTIVAR SESIÓN (Debe ir después de StaticFiles y antes de Routing)
app.UseSession();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=login}/{id?}");

app.Run();