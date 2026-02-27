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
        // En HomeController.cs
[HttpPost]
public async Task<IActionResult> ProcesarRegistro(string nombre, string apellido_paterno, string apellido_materno, string correo, string username, string password)
{
    try
    {
        var nuevoUsuario = new Usuario
        {
            nombre = nombre,
            apellido_paterno = apellido_paterno,
            apellido_materno = apellido_materno,
            correo = correo,
            username = username,
            password = password,
            rol = "cliente"
        };

        _context.usuarios.Add(nuevoUsuario);
        int filasAfetadas = await _context.SaveChangesAsync();

        if (filasAfetadas > 0)
        {
            TempData["Mensaje"] = "¡A huevo! Guardado en Supabase.";
            return RedirectToAction("login");
        }
        else
        {
            ViewBag.Error = "La base de datos dijo que no, pero no dio error. Revisa la conexión.";
            return View("registro");
        }
    }
    catch (Exception ex)
    {
        // Esto nos va a decir si Supabase rechazó el INSERT por el ENUM o por permisos
        ViewBag.Error = "Error real: " + (ex.InnerException?.Message ?? ex.Message);
        return View("registro");
    }
}
}
}
