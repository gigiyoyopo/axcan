// Mega Controlador AXCAN: Maneja Portal (Búsqueda/Estrellas), Admin SPA, Roles, AJAX Calendario, Reservas y SEGURIDAD GOOGLE.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth;
using axcan.Data; 
using axcan.Models; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq; 

namespace axcan.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =======================================================
        // EL BUSCADOR Y LAS ESTRELLAS (De tus compañeros)
        // =======================================================
        [HttpGet]
        public async Task<IActionResult> Index(string busqueda)
        {
            var empresasQuery = _context.empresas.AsQueryable();

            if (!string.IsNullOrEmpty(busqueda))
            {
                busqueda = busqueda.ToLower();
                empresasQuery = empresasQuery.Where(e => 
                    e.nombre_empresa.ToLower().Contains(busqueda) || 
                    e.rubro.ToLower().Contains(busqueda));
            }

            var listaEmpresas = await empresasQuery.ToListAsync();
            
            var empresasConEstrellas = new List<EmpresaConEstrellasDTO>();

            foreach (var emp in listaEmpresas)
            {
                var resenas = await _context.resenas.Where(r => r.id_empresa == emp.id_empresa).ToListAsync();
                double promedio = resenas.Any() ? resenas.Average(r => r.calificacion) : 0;

                empresasConEstrellas.Add(new EmpresaConEstrellasDTO {
                    Empresa = emp,
                    PromedioEstrellas = Math.Round(promedio, 1),
                    TotalResenas = resenas.Count
                });
            }

            ViewBag.EmpresasDestacadas = empresasConEstrellas;
            return View();
        }

        // =======================================================
        // PANEL ADMIN SPA PREMIUM (De tus compañeros)
        // =======================================================
        [Route("admin")]
        public async Task<IActionResult> Admin()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null) return RedirectToAction("login");

            var empresa = await _context.empresas.FirstOrDefaultAsync(e => e.id_administrador == userId);
            
            if (empresa != null) 
            {
                ViewBag.RolLocal = "administrador";
                ViewBag.Servicios = await _context.servicios.Where(s => s.id_empresa == empresa.id_empresa).ToListAsync();
                ViewBag.Personal = await (from s in _context.secretarios
                                      join u in _context.usuarios on s.id_usuario equals u.id_usuario
                                      where s.id_empresa == empresa.id_empresa
                                      select new { u.nombre, u.username, s.subrol, s.id_secretario }).ToListAsync();
                
                ViewBag.Horarios = await _context.horarios_negocio.Where(h => h.id_empresa == empresa.id_empresa).ToListAsync();
                return View(empresa);
            }

            var empleado = await _context.secretarios.FirstOrDefaultAsync(s => s.id_usuario == userId);
            if (empleado != null) 
            {
                var empresaEmpleado = await _context.empresas.FindAsync(empleado.id_empresa);
                ViewBag.RolLocal = empleado.subrol;
                ViewBag.Citas = await _context.citas.Where(c => c.id_empresa == empleado.id_empresa).ToListAsync();
                return View(empresaEmpleado);
            }

            return RedirectToAction("registronegocio");
        }

        // =======================================================
        // PLANTILLAS DINÁMICAS 
        // =======================================================
        [Route("reservar/{nombreUrl}")]
        public async Task<IActionResult> PaginaCliente(string nombreUrl)
        {
            var empresa = await _context.empresas
                .FirstOrDefaultAsync(e => e.nombre_empresa.Replace(" ", "-").ToLower() == nombreUrl.ToLower());

            if (empresa == null) return NotFound("Página no encontrada.");

            ViewBag.Servicios = await _context.servicios.Where(s => s.id_empresa == empresa.id_empresa && s.activo).ToListAsync();
            ViewBag.Personal = await (from s in _context.secretarios
                                      join u in _context.usuarios on s.id_usuario equals u.id_usuario
                                      where s.id_empresa == empresa.id_empresa && s.subrol == "prestador"
                                      select new { u.nombre, s.id_secretario }).ToListAsync();

            string nombreVista = empresa.id_plantilla switch
            {
                2 => "Plantilla_2",
                3 => "Plantilla_3",
                _ => "Plantilla_1"
            };

            return View(nombreVista, empresa);
        }

        // =======================================================
        // GESTIÓN DE NEGOCIO (Guardar Datos)
        // =======================================================
        [HttpPost]
        public async Task<IActionResult> GuardarEmpresa(Empresa e, IFormFile logoArchivo, string rubroElegido, string rubroOtro)
        {
            try {
                var userId = HttpContext.Session.GetInt32("UsuarioId");
                if (userId == null) return RedirectToAction("login");

                e.rubro = (rubroElegido == "Otros") ? rubroOtro : rubroElegido;

                if (logoArchivo != null && logoArchivo.Length > 0) {
                    e.logotipo_url = await GuardarFile(logoArchivo, "logos");
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
        public async Task<IActionResult> EditarNegocio(Empresa e, IFormFile logoArchivo, IFormFile bannerArchivo)
        {
            var empresaDb = await _context.empresas.FindAsync(e.id_empresa);
            if (empresaDb == null) return NotFound();

            empresaDb.nombre_empresa = e.nombre_empresa;
            empresaDb.color_tema = e.color_tema;
            empresaDb.id_plantilla = e.id_plantilla;

            if (logoArchivo != null) empresaDb.logotipo_url = await GuardarFile(logoArchivo, "logos");
            if (bannerArchivo != null) empresaDb.banner_url = await GuardarFile(bannerArchivo, "banners");

            _context.empresas.Update(empresaDb);
            await _context.SaveChangesAsync();
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<IActionResult> GuardarServicio(Servicio s)
        {
            _context.servicios.Add(s);
            await _context.SaveChangesAsync();
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<IActionResult> AgregarPersonal(string usernameBusqueda, string subrol, int idEmpresa)
        {
            var usuario = await _context.usuarios.FirstOrDefaultAsync(u => u.username == usernameBusqueda);
            if (usuario != null) {
                var nuevoStaff = new Secretario {
                    id_usuario = usuario.id_usuario,
                    id_empresa = idEmpresa,
                    subrol = subrol
                };
                _context.secretarios.Add(nuevoStaff);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<IActionResult> GuardarHorarios(int id_empresa, IFormCollection form)
        {
            var horariosAntiguos = _context.horarios_negocio.Where(h => h.id_empresa == id_empresa);
            _context.horarios_negocio.RemoveRange(horariosAntiguos);
            await _context.SaveChangesAsync();

            for (int i = 0; i < 7; i++)
            {
                var apertura = form[$"apertura_{i}"];
                var cierre = form[$"cierre_{i}"];
                var esDescanso = form[$"descanso_{i}"].Count > 0;

                var nuevoHorario = new HorarioNegocio
                {
                    id_empresa = id_empresa,
                    dia_semana = i,
                    hora_apertura = string.IsNullOrEmpty(apertura) ? TimeSpan.Zero : TimeSpan.Parse(apertura),
                    hora_cierre = string.IsNullOrEmpty(cierre) ? TimeSpan.Zero : TimeSpan.Parse(cierre),
                    es_descanso = esDescanso
                };
                _context.horarios_negocio.Add(nuevoHorario);
            }
            await _context.SaveChangesAsync();
            TempData["Exito"] = "Horarios guardados correctamente.";
            return RedirectToAction("Admin");
        }

        // =======================================================
        // LA API DEL CALENDARIO (AJAX DE AXEL)
        // =======================================================
        [HttpGet]
        public async Task<IActionResult> GetHorariosDisponibles(int idEmpresa, DateTime fecha)
        {
            int diaSemana = (int)fecha.DayOfWeek;
            
            var horarioDia = await _context.horarios_negocio
                .FirstOrDefaultAsync(h => h.id_empresa == idEmpresa && h.dia_semana == diaSemana);

            if (horarioDia == null || horarioDia.es_descanso)
                return Json(new { disponibles = new List<string>(), mensaje = "Día de descanso" });

            var citasOcupadas = await _context.citas
                .Where(c => c.id_empresa == idEmpresa && c.fecha_cita.Date == fecha.Date)
                .Select(c => c.hora_cita)
                .ToListAsync();

            var horasDisponibles = new List<string>();
            var horaActual = horarioDia.hora_apertura;

            while (horaActual < horarioDia.hora_cierre)
            {
                string horaFormateada = horaActual.ToString(@"hh\:mm");
                
                if (!citasOcupadas.Contains(horaFormateada))
                {
                    horasDisponibles.Add(horaFormateada);
                }
                horaActual = horaActual.Add(TimeSpan.FromMinutes(30));
            }

            return Json(new { disponibles = horasDisponibles });
        }

        [HttpPost]
        public async Task<IActionResult> AgendarCita(Cita nuevaCita)
        {
            nuevaCita.estatus = "Confirmada";
            nuevaCita.fecha_creacion = DateTime.Now;
            _context.citas.Add(nuevaCita);
            await _context.SaveChangesAsync();
            return Json(new { success = true, mensaje = "Cita agendada con éxito" });
        }

        [HttpGet]
        public async Task<JsonResult> GetDatosCalendario(int idEmpresa)
        {
            var servicios = await _context.servicios.Where(s => s.id_empresa == idEmpresa && s.activo).ToListAsync();
            var personal = await _context.secretarios.Where(s => s.id_empresa == idEmpresa && s.subrol == "prestador").ToListAsync();
            return Json(new { servicios = servicios, prestadores = personal });
        }

        // =======================================================
        // AUTENTICACIÓN Y PERFIL DE USUARIO (TU CÓDIGO RESTAURADO)
        // =======================================================
        public IActionResult login() => View();
        public IActionResult registro() => View();
        public IActionResult registronegocio() => View();
        public IActionResult acercade() => View();

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
                TempData["Mensaje"] = "¡Bienvenido a AXCAN! 🚀 Tu cuenta ha sido creada con éxito. Inicia sesión y descubre la forma más rápida de gestionar tus reservas.";
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

        // --- TU OBRA MAESTRA DE GOOGLE ---
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

                string correoPuro = payload.Email.ToLower();

                var user = await _context.usuarios.FirstOrDefaultAsync(u => u.correo.ToLower() == correoPuro);
                
                if (user == null)
                {
                    user = new Usuario 
                    {
                        username = correoPuro.Split('@')[0] + "_" + new Random().Next(100, 999), 
                        correo = correoPuro,
                        nombre = payload.GivenName ?? "Usuario",
                        apellido_paterno = payload.FamilyName ?? "",
                        apellido_materno = "", 
                        password = "EMPTY", 
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
                string detalleError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { success = false, message = detalleError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarPerfil(Usuario modeloModificado)
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null) return RedirectToAction("login");

            var usuarioOcupado = await _context.usuarios
                .AnyAsync(u => (u.username == modeloModificado.username || u.correo == modeloModificado.correo) && u.id_usuario != userId);

            if (usuarioOcupado)
            {
                TempData["Error"] = "El Nombre de Usuario o Correo ya están en uso.";
                return RedirectToAction("ConfiguracionDeCuenta"); 
            }

            var usuarioDb = await _context.usuarios.FindAsync(userId);
            if (usuarioDb != null)
            {
                usuarioDb.nombre = modeloModificado.nombre;
                usuarioDb.apellido_paterno = modeloModificado.apellido_paterno;
                usuarioDb.apellido_materno = modeloModificado.apellido_materno;
                usuarioDb.correo = modeloModificado.correo;
                usuarioDb.username = modeloModificado.username;
                
                if (!string.IsNullOrEmpty(modeloModificado.password)) 
                {
                    usuarioDb.password = modeloModificado.password;
                }

                _context.usuarios.Update(usuarioDb);
                await _context.SaveChangesAsync(); 
                
                HttpContext.Session.SetString("UsuarioNombre", usuarioDb.nombre);
                TempData["Exito"] = "Perfil actualizado correctamente.";
            }

            return RedirectToAction("ConfiguracionDeCuenta");
        }

        // ¡AQUÍ ESTÁ LA CORRECCIÓN DEL ERROR DE RENDER! Le agregamos "async Task<IActionResult>"
        public async Task<IActionResult> Logout() {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("login");
        }

        private async Task<string> GuardarFile(IFormFile file, string folder)
        {
            string name = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            using (var stream = new FileStream(Path.Combine(path, name), FileMode.Create)) {
                await file.CopyToAsync(stream);
            }
            return $"/{folder}/{name}";
        }
    }
    
    // CLASES DE APOYO (Tanto las de tus compañeros como la tuya de Google)
    public class EmpresaConEstrellasDTO
    {
        public Empresa Empresa { get; set; }
        public double PromedioEstrellas { get; set; }
        public int TotalResenas { get; set; }
    }

    public class GoogleToken
    {
        public string Credential { get; set; }
    }
}