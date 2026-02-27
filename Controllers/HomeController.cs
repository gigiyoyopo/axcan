using Microsoft.AspNetCore.Mvc;
using axcan.Data; 
using axcan.Models; 
using Microsoft.EntityFrameworkCore;

namespace axcan.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- MÉTODOS DE VISTA ---

        public IActionResult Index() => View();
        public IActionResult login() => View();
        public IActionResult registro() => View();
        public IActionResult acercade() => View();
        public IActionResult registronegocio() => View();

        [Route("admin")]
        public IActionResult Admin() => View();

        // --- LÓGICA DE REGISTRO DE USUARIO ---
        [HttpPost]
public async Task<IActionResult> ProcesarRegistro(Usuario u)
{
    try {
        u.rol = "usuario"; // <--- Todos empiezan desde abajo
        u.fecha_registro = DateTime.Now;
        
        _context.usuarios.Add(u);
        await _context.SaveChangesAsync();
        
        TempData["Mensaje"] = "¡Cuenta creada! Inicia sesión.";
        return RedirectToAction("login");
    }
    catch (Exception ex) {
        ViewBag.Error = "Error al crear cuenta: " + ex.Message;
        return View("registro");
    }
}

        // --- LÓGICA DE REGISTRO DE EMPRESA ---
        [HttpPost]
    [HttpPost]
public async Task<IActionResult> GuardarEmpresa(Empresa e)
{
    try {
        // 1. Guardamos la empresa
        _context.empresas.Add(e);
        
        // 2. Buscamos al usuario actual (el que está en sesión)
        var userId = HttpContext.Session.GetInt32("UsuarioId");
        var usuario = await _context.usuarios.FindAsync(userId);

        if (usuario != null) {
            usuario.rol = "administrador"; // ¡FELICIDADES, YA ES ADMIN!
            _context.usuarios.Update(usuario);
            
            // Actualizamos la sesión para que el menú cambie de inmediato
            HttpContext.Session.SetString("UsuarioRol", "administrador");
        }

        await _context.SaveChangesAsync();
        
        TempData["Mensaje"] = "¡Empresa registrada! Ahora tienes acceso al Panel de Administración.";
        return RedirectToAction("Admin");
    }
    catch (Exception ex) {
        ViewBag.Error = "No se pudo registrar la empresa: " + ex.Message;
        return View("registronegocio");
    }
}
   // 1. LOGIN: Para que el sistema "sepa" quién eres
[HttpPost]
public async Task<IActionResult> ProcesarLogin(string correo, string password)
{
    var user = await _context.usuarios
        .FirstOrDefaultAsync(u => u.correo == correo && u.password == password);

    if (user != null)
    {
        HttpContext.Session.SetInt32("UsuarioId", user.id_usuario);
        HttpContext.Session.SetString("UsuarioNombre", user.nombre);
        HttpContext.Session.SetString("UsuarioRol", user.rol); // usuario o administrador
        
        return RedirectToAction("Index");
    }
    ViewBag.Error = "Datos incorrectos";
    return View("login");
}

// 2. CONFIGURACIÓN: La vista que pediste para el icono de perfil
public async Task<IActionResult> ConfiguracionCuenta()
{
    var userId = HttpContext.Session.GetInt32("UsuarioId");
    if (userId == null) return RedirectToAction("login");

    var usuario = await _context.usuarios.FindAsync(userId);
    return View(usuario);
}
    }
}