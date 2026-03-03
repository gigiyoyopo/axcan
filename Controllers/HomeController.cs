using Microsoft.AspNetCore.Mvc;
using axcan.Data; 
using axcan.Models; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace axcan.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- 1. VISTAS PÚBLICAS ---
        public async Task<IActionResult> Index() 
        {
            var empresas = await _context.empresas.ToListAsync();
            return View(empresas);
        }

        public IActionResult login() => View();
        public IActionResult registro() => View();
        public IActionResult acercade() => View();
        public IActionResult registronegocio() => View();

        [Route("admin")]
        public IActionResult Admin() => View();

        // --- 2. LÓGICA DE USUARIOS ---

        [HttpPost]
        public async Task<IActionResult> ProcesarRegistro(Usuario u)
        {
            try {
                var existe = await _context.usuarios.AnyAsync(x => x.username == u.username);
                if (existe) {
                    ViewBag.Error = "Ese nombre de usuario ya existe.";
                    return View("registro");
                }
                u.rol = "usuario"; 
                u.fecha_registro = DateTime.Now;
                _context.usuarios.Add(u);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "¡Cuenta creada! Ya puedes iniciar sesión.";
                return RedirectToAction("login");
            }
            catch (Exception ex) {
                ViewBag.Error = "Error al registrar: " + ex.Message;
                return View("registro");
            }
        }
[HttpPost]
public async Task<IActionResult> ProcesarLogin(string username, string password)
{
    // Fix: Buscamos por username o correo para que siempre entre
    var user = await _context.usuarios
        .FirstOrDefaultAsync(u => (u.username == username || u.correo == username) && u.password == password);

    if (user != null) {
        HttpContext.Session.SetInt32("UsuarioId", user.id_usuario);
        HttpContext.Session.SetString("UsuarioNombre", user.nombre);
        HttpContext.Session.SetString("UsuarioRol", user.rol);
        return RedirectToAction("Index");
    }
    
    ViewBag.Error = "Usuario o contraseña incorrectos."; 
    return View("login");
}
        // --- 3. LÓGICA DE NEGOCIO (EL ASCENSO) ---

       [HttpPost]
public async Task<IActionResult> GuardarEmpresa(Empresa e, IFormFile logoArchivo)
{
    try {
        var userId = HttpContext.Session.GetInt32("UsuarioId");
        if (userId == null) return RedirectToAction("login");

        // Lógica para el Logo (Guardado local por ahora)
        if (logoArchivo != null && logoArchivo.Length > 0) {
            string nombreArchivo = Guid.NewGuid().ToString() + "_" + logoArchivo.FileName;
            string ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagenesaxcan", nombreArchivo);
            using (var stream = new FileStream(ruta, FileMode.Create)) {
                await logoArchivo.CopyToAsync(stream);
            }
            e.logotipo_url = "/imagenesaxcan/" + nombreArchivo;
        }

        e.id_administrador = userId;
        _context.empresas.Add(e);
        
        // Ascenso a Admin
        var usuario = await _context.usuarios.FindAsync(userId);
        if (usuario != null) {
            usuario.rol = "administrador";
            _context.usuarios.Update(usuario);
            HttpContext.Session.SetString("UsuarioRol", "administrador");
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Admin");
    }
    catch (Exception ex) {
        ViewBag.Error = "Error: " + ex.Message;
        return View("registronegocio");
    }
}
}}