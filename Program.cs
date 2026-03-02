using Microsoft.EntityFrameworkCore;
using axcan.Data;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// 1. SERVICIOS
builder.Services.AddControllersWithViews();

// Nota: En Render, tu variable de entorno debe llamarse "ConnectionStrings__DefaultConnection" 
// (con doble guion bajo) para que GetConnectionString la detecte automáticamente.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// CONFIGURACIÓN DE GOOGLE - BLINDADA PARA RENDER
builder.Services.AddAuthentication(options => {
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Google";
})
.AddCookie("Cookies", options => {
    options.Cookie.SameSite = SameSiteMode.Lax; 
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
})
.AddGoogle("Google", options => {
    // Falla rápida: Si no encuentra las credenciales, la app te avisará inmediatamente en los logs de Render
    options.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") 
        ?? throw new InvalidOperationException("Falta la variable de entorno GOOGLE_CLIENT_ID");
    options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") 
        ?? throw new InvalidOperationException("Falta la variable de entorno GOOGLE_CLIENT_SECRET");
    
    // Mantenemos tu blindaje de redirección como capa extra de seguridad
    options.Events.OnRedirectToAuthorizationEndpoint = context =>
    {
        var redirectUri = context.RedirectUri.Replace("http://", "https://");
        context.Response.Redirect(redirectUri);
        return Task.CompletedTask;
    };
});

var app = builder.Build();

// 2. MIDDLEWARE (EL ORDEN AQUÍ ES DE VIDA O MUERTE PARA RENDER)

// A. Configurar las opciones del Proxy
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
// CRÍTICO: Limpiar las redes conocidas para que .NET acepte los encabezados del balanceador de Render
forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();

// B. ESTO DEBE SER LO PRIMERO EN EL PIPELINE
app.UseForwardedHeaders(forwardedHeadersOptions);

// C. Forzamos el esquema HTTPS como red de seguridad (ya no forzamos el Host para no romper los Health Checks de Render)
app.Use((context, next) =>
{
    context.Request.Scheme = "https";
    return next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Buena práctica de seguridad recomendada en producción
}

app.UseStaticFiles();
app.UseRouting();

// El orden de estos dos no se puede cambiar
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=login}/{id?}");

app.Run();