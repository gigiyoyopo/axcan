using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth;
using axcan.Data; 
using axcan.Models; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

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
            var empresas = await _context.empresas.ToListAsync();
            return View(empresas);
        }

        public IActionResult login() => View();
        public IActionResult registro() => View();
        public IActionResult acercade() => View();
        public IActionResult registronegocio() => View();

        [Route("admin")]
        public async Task<IActionResult> Admin()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null) return RedirectToAction("login");

            var empresa = await _context.empresas.FirstOrDefaultAsync(e => e.id_administrador == userId);
            if (empresa == null) return RedirectToAction("registronegocio");

            return View(empresa);
        }

        public async Task<IActionResult> ConfiguracionDeCuenta()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null) return RedirectToAction("login");

            var usuario = await _context.usuarios.FindAsync(userId);
            return View(usuario);
        }

        // --- 2. LÓGICA DE USUARIOS MANUAL ---

        [HttpPost]
        public async Task<IActionResult> ProcesarRegistro(Usuario u)
        {
            try {
                var existe = await _context.usuarios.AnyAsync(x => x.username == u.username);
                if (existe) {
                    ViewBag.Error = "Ese nombre de usuario ya existe.";
                    return View("registro");
                }
                u.rol = "usuario";
                u.fecha_registro = DateTime.Now;
                _context.usuarios.Add(u);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "¡Cuenta creada! Ya puedes iniciar sesión.";
                return RedirectToAction("login");
            }
            catch (Exception ex) {
                ViewBag.Error = "Error al registrar: " + ex.Message;
                return View("registro");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarLogin(string username, string password)
        {
            var user = await _context.usuarios
                .FirstOrDefaultAsync(u => (u.username == username || u.correo == username) && u.password == password);

            if (user != null) {
                HttpContext.Session.SetInt32("UsuarioId", user.id_usuario);
                HttpContext.Session.SetString("UsuarioNombre", user.nombre);
                HttpContext.Session.SetString("UsuarioRol", user.rol);
                return RedirectToAction("Index");
            }
            
            ViewBag.Error = "Usuario o contraseña incorrectos."; 
            return View("login");
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarPerfil(Usuario u)
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null) return RedirectToAction("login");

            try {
                var usuarioDb = await _context.usuarios.FindAsync(userId);
                if (usuarioDb != null) {
                    usuarioDb.nombre = u.nombre;
                    usuarioDb.apellido_paterno = u.apellido_paterno;
                    usuarioDb.apellido_materno = u.apellido_materno;
                    usuarioDb.username = u.username;
                    usuarioDb.correo = u.correo;

                    if (!string.IsNullOrEmpty(u.password)) {
                        usuarioDb.password = u.password;
                    }

                    _context.usuarios.Update(usuarioDb);
                    await _context.SaveChangesAsync();
                    
                    HttpContext.Session.SetString("UsuarioNombre", u.nombre);
                    TempData["Mensaje"] = "Perfil actualizado con éxito.";
                }
            }
            catch (Exception ex) {
                TempData["Error"] = "Error: " + ex.Message;
            }
            return RedirectToAction("ConfiguracionDeCuenta");
        }

        // --- EL PUENTE DE SEGURIDAD CON GOOGLE ---

        [HttpPost]
        public async Task<IActionResult> AutenticarConGoogle([FromBody] GoogleToken model)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { "1058925398660-ovi8nq4pj2a0qtn7kmelganfug5lu008.apps.googleusercontent.com" } 
                };
                
                var payload = await GoogleJsonWebSignature.ValidateAsync(model.Credential, settings);

                var user = await _context.usuarios.FirstOrDefaultAsync(u => u.correo == payload.Email);
                
                if (user == null)
                {
                    user = new Usuario 
                    {
                        username = payload.Email.Split('@')[0], 
                        correo = payload.Email,
                        nombre = payload.GivenName ?? "Usuario",
                        apellido_paterno = payload.FamilyName ?? "",
                        password = "", 
                        rol = "usuario",
                        fecha_registro = DateTime.Now
                    };
                    _context.usuarios.Add(user);
                    await _context.SaveChangesAsync();
                }

                HttpContext.Session.SetInt32("UsuarioId", user.id_usuario);
                HttpContext.Session.SetString("UsuarioNombre", user.nombre);
                HttpContext.Session.SetString("UsuarioRol", user.rol);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, payload.Subject),
                    new Claim(ClaimTypes.Name, payload.Name),
                    new Claim(ClaimTypes.Email, payload.Email)
                };
                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties { IsPersistent = true });

                return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Error al validar token: " + ex.Message });
            }
        }

        // --- 3. LÓGICA DE NEGOCIO ---

        [HttpPost]
        public async Task<IActionResult> GuardarEmpresa(Empresa e, IFormFile logoArchivo, string rubroElegido, string rubroOtro)
        {
            try {
                var userId = HttpContext.Session.GetInt32("UsuarioId");
                if (userId == null) return RedirectToAction("login");

                e.rubro = (rubroElegido == "Otros") ? rubroOtro : rubroElegido;

                if (logoArchivo != null && logoArchivo.Length > 0) {
                    e.logotipo_url = await GuardarArchivoPersonalizado(logoArchivo, "logos");
                }

                e.id_administrador = userId.GetValueOrDefault();
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
                ViewBag.Error = "Error al registrar: " + ex.Message;
                return View("registronegocio"); 
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditarNegocio(Empresa e, IFormFile logoArchivo, IFormFile nuevoBanner, string rubroElegido, string rubroOtro)
        {
            try {
                var empresaDb = await _context.empresas.FindAsync(e.id_empresa);
                if (empresaDb == null) return NotFound();

                empresaDb.nombre_empresa = e.nombre_empresa;
                empresaDb.rubro = (rubroElegido == "Otros") ? rubroOtro : rubroElegido;
                empresaDb.ubicacion_lat = e.ubicacion_lat;
                empresaDb.ubicacion_lng = e.ubicacion_lng;

                if (logoArchivo != null && logoArchivo.Length > 0) {
                    empresaDb.logotipo_url = await GuardarArchivoPersonalizado(logoArchivo, "logos");
                }

                _context.empresas.Update(empresaDb);
                await _context.SaveChangesAsync();
                
                TempData["Mensaje"] = "¡Negocio actualizado con éxito!";
                return RedirectToAction("Admin");
            }
            catch (Exception ex) {
                ViewBag.Error = "Error al actualizar: " + ex.Message;
                return RedirectToAction("Admin");
            }
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("login");
        }

        private async Task<string> GuardarArchivoPersonalizado(IFormFile archivo, string carpeta)
        {
            string nombre = Guid.NewGuid().ToString() + Path.GetExtension(archivo.FileName);
            string rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", carpeta);
            
            if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);

            string rutaCompleta = Path.Combine(rutaCarpeta, nombre);
            using (var stream = new FileStream(rutaCompleta, FileMode.Create)) {
                await archivo.CopyToAsync(stream);
            }
            return $"/{carpeta}/{nombre}";
        }
    
        [Route("reservar/{nombreUrl}")]
        public async Task<IActionResult> PaginaReserva(string nombreUrl)
        {
            var empresa = await _context.empresas
                .FirstOrDefaultAsync(e => e.nombre_empresa.Replace(" ", "-").ToLower() == nombreUrl.ToLower());

            if (empresa == null) return NotFound();

            return View("Plantilla_" + empresa.id_plantilla, empresa);}
    }}