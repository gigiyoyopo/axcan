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
public async Task<IActionResult> ProcesarRegistro(string nombre, string apellido_paterno, string apellido_materno, string correo, string username, string password)
{
    try
    {
        // 1. Creamos el usuario con los datos que llegan del formulario
        var nuevoUsuario = new Usuario
        {
            nombre = nombre,
            apellido_paterno = apellido_paterno,
            apellido_materno = apellido_materno,
            correo = correo,
            username = username,
            password = password, // Luego le metemos hash para que esté blindado
            rol = "cliente" // El valor de tu ENUM en minúsculas
        };

        // 2. Guardamos en la base de datos de Supabase
        _context.usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();

        // 3. ¡ESTO ES LO QUE FALTA! Redirigir para que no salga página vacía
        TempData["Mensaje"] = "¡Registro exitoso, cawn! Ya puedes entrar.";
        return RedirectToAction("login"); 
    }
    catch (Exception ex)
    {
        // Si algo truena, nos regresa al registro y nos dice qué pasó
        ViewBag.Error = "No se pudo guardar: " + (ex.InnerException?.Message ?? ex.Message);
        return View("registro"); 
    }
}
}
}
