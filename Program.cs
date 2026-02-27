using Microsoft.EntityFrameworkCore;
using axcan.Data;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// 1. SERVICIOS
builder.Services.AddControllersWithViews();

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
    options.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? "";
    options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") ?? "";
    
    // ESTA ES LA CLAVE: Forzamos que la URL de redirección sea siempre HTTPS
    options.Events.OnRedirectToAuthorizationEndpoint = context =>
    {
        var redirectUri = context.RedirectUri.Replace("http://", "https://");
        context.Response.Redirect(redirectUri);
        return Task.CompletedTask;
    };
});

var app = builder.Build();

// 2. MIDDLEWARE (ORDEN CRÍTICO PARA EVITAR OVERFLOW Y MISMATCH)

// Forzamos a que la app crea que es HTTPS antes de procesar nada más
app.Use((context, next) =>
{
    context.Request.Scheme = "https";
    context.Request.Host = new HostString("axcan.onrender.com");
    return next();
});

// Configuramos los encabezados reenviados para que confíe en el Proxy de Render
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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