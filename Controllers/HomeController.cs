using Microsoft.AspNetCore.Mvc;

namespace axcan.Controllers
{
    public class HomeController : Controller
    {
        // 1. Pantalla de Inicio (Carrousel)
        public IActionResult Index()
        {
            return View();
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
    }
}
