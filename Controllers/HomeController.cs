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
public async Task<IActionResult> ProcesarRegistro(string nombre, string apellido_paterno, string apellido_materno, string email, string username, string password)
{
    // 1. Validación rápida
    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nombre))
    {
        ViewBag.Error = "llena todos los campos.";
        return View("registro");
    }

    try
    {
        // 2. Creamos el objeto Usuario con los campos exactos de tu SQL
        var nuevoUsuario = new Usuario
        {
            nombre = nombre,
            apellido_paterno = apellido_paterno,
            apellido_materno = apellido_materno,
            correo = email, // Tu SQL usa 'correo'
            username = username,
            password = password,
            rol = "cliente", // El valor del ENUM en minúsculas
            fecha_registro = DateTime.Now
        };

        // 3. Guardar en Supabase
        _context.usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();

        TempData["Success"] = "¡Ya estás registrado! Inicia sesión.";
        return RedirectToAction("login");
    }
    catch (Exception ex)
    {
        // Esto te dirá qué falló específicamente (ej. correo repetido)
        ViewBag.Error = "Error: " + (ex.InnerException?.Message ?? ex.Message);
        return View("registro");
    }
}}
}
