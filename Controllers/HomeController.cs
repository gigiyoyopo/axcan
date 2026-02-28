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

        // --- VISTAS PÚBLICAS ---
        public IActionResult Index() => View();
        public IActionResult login() => View();
        public IActionResult registro() => View();
        public IActionResult acercade() => View();
        public IActionResult registronegocio() => View();

        [Route("admin")]
        public IActionResult Admin() => View();

        // --- LÓGICA DE USUARIOS ---

        [HttpPost]
        public async Task<IActionResult> ProcesarRegistro(Usuario u)
        {
            try {
                u.rol = "usuario"; // Todos nacen como usuario
                u.fecha_registro = DateTime.Now;
                _context.usuarios.Add(u);
                await _context.SaveChangesAsync();
                
                TempData["Mensaje"] = "¡Cuenta creada! Inicia sesión.";
                return RedirectToAction("login");
            }
            catch (Exception ex) {
                ViewBag.Error = "Error: " + ex.Message;
                return View("registro");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarLogin(string correo, string password)
        {
            var user = await _context.usuarios
                .FirstOrDefaultAsync(u => u.correo == correo && u.password == password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UsuarioId", user.id_usuario);
                HttpContext.Session.SetString("UsuarioNombre", user.nombre);
                HttpContext.Session.SetString("UsuarioRol", user.rol);
                return RedirectToAction("Index");
            }
            ViewBag.Error = "Datos incorrectos";
            return View("login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("login");
        }

        // --- LÓGICA DE NEGOCIO (EL ASCENSO) ---

        [HttpPost]
        public async Task<IActionResult> GuardarEmpresa(Empresa e)
        {
            try {
                var userId = HttpContext.Session.GetInt32("UsuarioId");
                if (userId == null) return RedirectToAction("login");

                e.id_administrador = userId; 
                _context.empresas.Add(e);
                
                var usuario = await _context.usuarios.FindAsync(userId);
                if (usuario != null) {
                    usuario.rol = "administrador"; // ASCENSO AUTOMÁTICO
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

        public async Task<IActionResult> ConfiguracionCuenta()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null) return RedirectToAction("login");

            var usuario = await _context.usuarios.FindAsync(userId);
            return View(usuario);
        }
    }
}