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
public async Task<IActionResult> GuardarEmpresa(Empresa e, IFormFile logoArchivo, string rubroElegido, string rubroOtro)
{
    try 
    {
        var userId = HttpContext.Session.GetInt32("UsuarioId");
        if (userId == null) return RedirectToAction("login");

        // --- LÓGICA DEL RUBRO (OTROS) ---
        // Si eligió "Otros", usamos lo que escribió en el cuadro de texto
        e.rubro = (rubroElegido == "Otros") ? rubroOtro : rubroElegido;

        // --- LÓGICA DEL LOGO (PUNTO 4) ---
        if (logoArchivo != null && logoArchivo.Length > 0)
        {
            string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(logoArchivo.FileName);
            string rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/logos");
            
            // Si la carpeta no existe (como en un server nuevo), la creamos
            if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);

            string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await logoArchivo.CopyToAsync(stream);
            }

            e.logotipo_url = "/logos/" + nombreArchivo;
        }

        // --- GUARDADO EN DB ---
        e.id_administrador = userId;
        _context.empresas.Add(e);

        // ASCENSO DE RANGO
        var usuario = await _context.usuarios.FindAsync(userId);
        if (usuario != null) {
            usuario.rol = "administrador";
            _context.usuarios.Update(usuario);
            HttpContext.Session.SetString("UsuarioRol", "administrador");
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Admin");
    }
    catch (Exception ex)
    {
        ViewBag.Error = "Error al registrar: " + ex.Message;
        return View("registronegocio");
    }

}
}}