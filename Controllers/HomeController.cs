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

        // --- 1. VISTAS PÚBLICAS ---

        public async Task<IActionResult> Index() 
        {
            // Carga la lista de empresas para las Cards del Index
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
            try 
            {
                // Verifica si el nombre de usuario ya existe
                var existe = await _context.usuarios.AnyAsync(x => x.username == u.username);
                if (existe)
                {
                    ViewBag.Error = "Ese nombre de usuario ya está ocupado.";
                    return View("registro");
                }

                u.rol = "usuario"; 
                u.fecha_registro = DateTime.Now;
                _context.usuarios.Add(u);
                await _context.SaveChangesAsync();
                
                TempData["Mensaje"] = "¡Cuenta creada! Ya puedes iniciar sesión.";
                return RedirectToAction("login");
            }
            catch (Exception ex) 
            {
                ViewBag.Error = "Error al registrar: " + ex.Message;
                return View("registro");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarLogin(string username, string password)
        {
            var user = await _context.usuarios
                .FirstOrDefaultAsync(u => u.username == username && u.password == password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UsuarioId", user.id_usuario);
                HttpContext.Session.SetString("UsuarioNombre", user.nombre);
                HttpContext.Session.SetString("UsuarioRol", user.rol);
                return RedirectToAction("Index");
            }
            
            ViewBag.Error = "Usuario o contraseña incorrectos.";
            return View("login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("login");
        }

        // --- 3. LÓGICA DE NEGOCIO (EL ASCENSO) ---

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
                    usuario.rol = "administrador"; 
                    _context.usuarios.Update(usuario);
                    HttpContext.Session.SetString("UsuarioRol", "administrador");
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Admin");
            }
            catch (Exception ex) {
                ViewBag.Error = "Error al registrar negocio: " + ex.Message;
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