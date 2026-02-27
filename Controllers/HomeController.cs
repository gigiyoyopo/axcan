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
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "¡A huevo! Guardado en Supabase.";
                return RedirectToAction("login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error real: " + (ex.InnerException?.Message ?? ex.Message);
                return View("registro");
            }
        }

        // --- LÓGICA DE REGISTRO DE EMPRESA ---
        [HttpPost]
        public async Task<IActionResult> GuardarEmpresa(string nombre_empresa, string rubro, string latitud, string longitud)
        {
            try
            {
                var nuevaEmpresa = new Empresa
                {
                    nombre_empresa = nombre_empresa,
                    rubro = rubro,
                    ubicacion_lat = decimal.Parse(latitud),
                    ubicacion_lng = decimal.Parse(longitud),
                    id_administrador = 1 // Temporal
                };

                _context.empresas.Add(nuevaEmpresa);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "¡Empresa registrada con éxito!";
                return RedirectToAction("Admin");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al guardar empresa: " + ex.Message;
                return View("registronegocio");
            }
        
    } // Cierre de la Clase
    [HttpPost]
public async Task<IActionResult> ProcesarLogin(string correo, string password)
{
    // 1. Buscamos al usuario por correo
    var usuario = await _context.usuarios
        .FirstOrDefaultAsync(u => u.correo == correo && u.password == password);

    if (usuario != null)
    {
        // 2. Guardamos sus datos en la Sesión (para que el menú sepa quién es)
        HttpContext.Session.SetString("UsuarioNombre", usuario.nombre);
        HttpContext.Session.SetInt32("UsuarioId", usuario.id_usuario);
        HttpContext.Session.SetString("UsuarioRol", usuario.rol);

        TempData["Mensaje"] = $"¡Qué onda, {usuario.nombre}! Bienvenido de vuelta.";
        return RedirectToAction("Admin");
    }
    else
    {
        ViewBag.Error = "Correo o contraseña incorrectos, cawn. Intenta de nuevo.";
        return View("login");
    }

} // Cierre del Namespace
    }
}