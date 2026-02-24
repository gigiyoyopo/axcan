using Microsoft.AspNetCore.Mvc;

namespace axcan.Controllers
{
    public class HomeController : Controller
    {
        // 1. Pantalla de Inicio (Carrousel)
        public IActionResult Index()
        {
          return RedirectToAction("Admin");
        }

        // 2. Pantalla de Login (Nuestra nueva pantalla principal)
        public IActionResult login()
        {
            return View();
        }

        // 3. Pantalla de Registro
        public IActionResult registro()
        {
            return View();
        }

        // 4. Pantalla de Acerca De
        public IActionResult acercade()
        {
            return View();
        }

        // 5. Pantalla de Registro de Negocio
        public IActionResult registronegocio()
        {
            return View();
        }
// Esto obliga a que si escribes axcan.onrender.com/admin entres directo
[Route("admin")]
public IActionResult Admin()
{
    // Usamos la ruta completa para que .NET no se pierda
    return View("~/Views/Home/Admin.cshtml");
}
    }}
