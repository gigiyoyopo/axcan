using Microsoft.AspNetCore.Mvc;
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

        // --- 2. LÓGICA DE USUARIOS ---

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

        // --- 3. LÓGICA DE NEGOCIO (CRM Y PLANTILLAS) ---

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
           int idLimpio = userId ?? 0;
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
                return View("registron  egocio");
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

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("login");
        }

        // Método auxiliar para guardar recursos de Axel
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
    // Buscamos el negocio por su nombre o ID
    var empresa = await _context.empresas
        .FirstOrDefaultAsync(e => e.nombre_empresa.Replace(" ", "-").ToLower() == nombreUrl.ToLower());

    if (empresa == null) return NotFound();

    // Pasamos los datos de personalización (color, logo, banner, plantilla) a la vista
 
    return View("Plantilla_" + empresa.id_plantilla, empresa); 
}
//--ver hoariors disponibles 
[HttpGet]
public async Task<JsonResult> GetHorariosDisponibles(int idEmpresa, int diaSeleccionado)
{
    // Buscamos el horario configurado para ese día de la semana
    var horario = await _context.horarios_negocio
        .FirstOrDefaultAsync(h => h.id_empresa == idEmpresa && h.dia_semana == diaSeleccionado);

    if (horario == null || horario.es_dia_descanso)
    {
        return Json(new { disponible = false });
    }

    return Json(new { 
        disponible = true, 
        apertura = horario.hora_apertura.ToString(@"hh\:mm"), 
        cierre = horario.hora_cierre.ToString(@"hh\:mm") 
    });
}
}
}