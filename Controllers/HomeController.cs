using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth;

namespace axcan.Controllers
{
    public class HomeController : Controller
    {
        // 1. Pantalla de Inicio
        public IActionResult Index()
        {
            return RedirectToAction("Admin");
        }

        // 2. Pantalla de Login 
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

        // 6. Pantalla de Admin (¡AHORA CON CANDADO DE SEGURIDAD!)
        [Authorize] // Si no hay cookie de .NET, te rebota automáticamente al login
        [Route("admin")]
        public IActionResult Admin()
        {
            return View("~/Views/Home/Admin.cshtml");
        }

        // --- EL PUENTE DE SEGURIDAD ---
        // Esta es la puerta que recibe el token desde JavaScript
        [HttpPost]
        public async Task<IActionResult> AutenticarConGoogle([FromBody] GoogleToken model)
        {
            try
            {
                // 1. Validar el token con los servidores de Google
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    // Tu Client ID exacto para asegurar que el token es para Axcan
                    Audience = new List<string>() { "1058925398660-ovi8nq4pj2a0qtn7kmelganfug5lu008.apps.googleusercontent.com" } 
                };
                
                var payload = await GoogleJsonWebSignature.ValidateAsync(model.Credential, settings);

                // 2. Crear la "Identificación" del usuario para .NET
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, payload.Subject),
                    new Claim(ClaimTypes.Name, payload.Name),
                    new Claim(ClaimTypes.Email, payload.Email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Para que el usuario no tenga que loguearse a cada rato
                };

                // 3. ¡Crear la Cookie oficial de sesión en .NET!
                await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity), authProperties);

                // 4. Avisarle a JavaScript que todo fue un éxito
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Error al validar token: " + ex.Message });
            }
        }
    }

    // Modelo sencillo para recibir el token en formato JSON
    public class GoogleToken
    {
        public string Credential { get; set; }
    }
}