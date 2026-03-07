using Microsoft.EntityFrameworkCore;
using axcan.Data;
using Microsoft.AspNetCore.HttpOverrides;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

// =================================================================
// 1. FASE DE SERVICIOS (TODO DEBE IR ANTES DEL BUILD)
// =================================================================

builder.Services.AddControllersWithViews();

// A. Base de Datos (Rescatamos la variable que faltaba)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions => {
        npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    }));

// B. Sesiones (Para que la lógica de tus compañeros funcione)
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// C. Seguridad y Cookies (Blindado para producción)
builder.Services.AddAuthentication(options => {
    options.DefaultScheme = "Cookies";
})
.AddCookie("Cookies", options => {
    options.LoginPath = "/Home/login"; // Si alguien intenta entrar sin permiso, lo manda aquí
    options.Cookie.SameSite = SameSiteMode.Lax; 
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
});

// =================================================================
// --- LA LÍNEA SAGRADA --- (No mover de aquí)
var app = builder.Build();
// =================================================================


// --- EL CHISMOSO (Para ver si Supabase conecta en los logs de Render) ---
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

// =================================================================
// 2. FASE DE MIDDLEWARE (EL ORDEN AQUÍ ES DE VIDA O MUERTE)
// =================================================================

// A. Configuración del Proxy (Para que Render entienda el tráfico)
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardedHeadersOptions);

// B. Forzar HTTPS
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

app.UseStaticFiles();
app.UseRouting();

// C. ¡EL SALVAVIDAS! (Activa la memoria de sesiones de tus compañeros)
app.UseSession();

// D. Activar Seguridad (Siempre después de UseRouting y UseSession)
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=login}/{id?}");

app.Run();