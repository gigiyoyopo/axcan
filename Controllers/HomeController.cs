using Microsoft.AspNetCore.Mvc;
using axcan.Data; // Asegúrate que este sea el namespace de tu DbContext
using axcan.Models; // Asegúrate que este sea el namespace de tus modelos
using Microsoft.EntityFrameworkCore;

namespace axcan.Controllers
{
    public class HomeController : Controller
    {
        // 1. Inyectamos el contexto de la base de datos
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- MÉTODOS DE VISTA ---

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult login()
        {
            return View();
        }

        public IActionResult registro()
        {
            return View();
        }

        public IActionResult acercade()
        {
            return View();
        }

        public IActionResult registronegocio()
        {
            return View();
        }

        [Route("admin")]
        public IActionResult Admin()
        {
            return View();
        }

        // --- LÓGICA DE REGISTRO ---

        [HttpPost]
        public async Task<IActionResult> ProcesarRegistro(string fullname, string email, string username, string password)
        {
            // Validar que no lleguen vacíos
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullname))
            {
                ViewBag.Error = "No dejes campos vacíos, cawn.";
                return View("registro");
            }

            try
            {
                // 2. Creamos el objeto con el modelo 'Usuario' (el que acordamos)
                var nuevoUsuario = new Usuario
                {
                    fullname = fullname,
                    email = email,
                    username = username,
                    password = password, // En el futuro usaremos cifrado
                    rol = "Cliente",
                    created_at = DateTime.UtcNow
                };

                // 3. Agregar a la tabla 'usuarios' en Supabase
                // OJO: Asegúrate que en tu ApplicationDbContext tengas: public DbSet<Usuario> usuarios { get; set; }
                _context.usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                // 4. Éxito: Mandar al login
                TempData["Success"] = "¡Registro exitoso! Ya eres parte de la manada AXCAN.";
                return RedirectToAction("login");
            }
            catch (Exception ex)
            {
                // Si sale error (ej. correo duplicado), lo atrapamos aquí
                ViewBag.Error = "Error al conectar con Supabase: " + ex.Message;
                return View("registro");
            }
        }
    }
}
