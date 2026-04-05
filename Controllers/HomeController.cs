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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using axcan.Data;
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
        // PORTAL Y BÚSQUEDA
        // =======================================================
        [HttpGet]
        public async Task<IActionResult> Index(string? busqueda)
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            ViewBag.EsStaff = await _context.secretarios.AnyAsync(s => s.id_usuario == userId);
            
            var empresasQuery = _context.empresas.AsQueryable();
            if (!string.IsNullOrEmpty(busqueda))
            {
                var busquedaLower = busqueda.ToLower();
                empresasQuery = empresasQuery.Where(e => 
                    e.nombre_empresa.ToLower().Contains(busquedaLower) || 
                    e.rubro.ToLower().Contains(busquedaLower));
            }

            var listaEmpresas = await empresasQuery.ToListAsync();
            var empresasConEstrellas = new List<EmpresaConEstrellasDTO>();
            
            foreach (var emp in listaEmpresas)
            {
                var resenas = await _context.resenas
                    .Where(r => r.id_empresa == emp.id_empresa)
                    .ToListAsync() ?? new List<Resena>();

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

        public IActionResult AcercaDe()
        {
            return View(); 
        }

        // =======================================================
        // AUTENTICACIÓN: REGISTRO, LOGIN Y GOOGLE
        // =======================================================
        [HttpGet]
        public IActionResult registro() => View();
[HttpPost]
public async Task<IActionResult> ProcesarRegistro(Usuario u)
{
    try 
    {
        // 1. Validar campos vacíos manualmente antes de ir a la BD
        if (string.IsNullOrEmpty(u.username) || string.IsNullOrEmpty(u.correo) || string.IsNullOrEmpty(u.password))
        {
            ViewBag.Error = "Todos los campos son obligatorios.";
            return View("registro");
        }

        // 2. Validar repetidos
        var existeUser = await _context.usuarios.AnyAsync(x => x.username == u.username);
        var existeCorreo = await _context.usuarios.AnyAsync(x => x.correo == u.correo);
        
        if (existeUser) {
            ViewBag.Error = "El nombre de usuario ya existe. Intenta con otro.";
            return View("registro");
        }
        if (existeCorreo) {
            ViewBag.Error = "Este correo ya está registrado. ¿Olvidaste tu contraseña?";
            return View("registro");
        }

        u.rol = "usuario";
        u.fecha_registro = DateTime.Now;
        
        ModelState.Clear();
        _context.usuarios.Add(u);
        await _context.SaveChangesAsync();
        
        TempData["Mensaje"] = "¡Cuenta creada! Ya puedes iniciar sesión.";
        return RedirectToAction("login");
    }
    catch (Exception ex) 
    {
        ViewBag.Error = "Error inesperado: " + (ex.InnerException?.Message ?? ex.Message);
        return View("registro");
    }
}
 [HttpGet]
        public IActionResult login() => View();

        [HttpPost]
        public async Task<IActionResult> ProcesarLogin(string username, string password)
        {
            var user = await _context.usuarios
                .FirstOrDefaultAsync(u => (u.username == username || u.correo == username) && u.password == password);

            if (user != null) {
                HttpContext.Session.SetInt32("UsuarioId", user.id_usuario);
                HttpContext.Session.SetString("UsuarioNombre", user.nombre ?? "Usuario");
                
                var tieneEmpresa = await _context.empresas.AnyAsync(e => e.id_administrador == user.id_usuario);
                string rolFinal = tieneEmpresa ? "admin" : (user.rol ?? "usuario");
                HttpContext.Session.SetString("UsuarioRol", rolFinal);

                if (rolFinal == "admin") return RedirectToAction("Admin");
                return RedirectToAction("Index");
            }
            
            ViewBag.Error = "Usuario o contraseña incorrectos."; 
            return View("login");
        }

        // ---> API DE GOOGLE RESTAURADA <---
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

                var tieneEmpresa = await _context.empresas.AnyAsync(e => e.id_administrador == user.id_usuario);
                string rolFinal = tieneEmpresa ? "admin" : (user.rol ?? "usuario");
                HttpContext.Session.SetString("UsuarioRol", rolFinal);

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

        // =======================================================
        // PERFIL DEL USUARIO
        // =======================================================
        [HttpGet("mi-perfil")]
        [ActionName("ConfiguracionDeCuenta")] 
        public async Task<IActionResult> MiPerfil()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null) return RedirectToAction("login");

            var usuario = await _context.usuarios.FindAsync(userId);
            if (usuario == null) return RedirectToAction("login");

            ViewBag.MisCitas = await (from c in _context.citas
                                     join e in _context.empresas on c.id_empresa equals e.id_empresa
                                     join ex in _context.expedientes on c.id_expediente equals ex.id_expediente
                                     where c.id_usuario_tramito == userId
                                     orderby c.fecha descending
                                     select new { c.id_cita, c.fecha, c.hora, c.estatus, e.nombre_empresa, ex.folio }).ToListAsync();

            return View(usuario);
        }

        // ---> ACTUALIZAR PERFIL RESTAURADO <---
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

        // =======================================================
        // ADMINISTRACIÓN Y STAFF (Admin, Secretario)
        // =======================================================
        [HttpGet("admin")]
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
                                         join us in _context.usuarios on s.id_usuario equals us.id_usuario
                                         where s.id_empresa == empresa.id_empresa
                                         select new { us.nombre, us.username, s.subrol, s.id_secretario }).ToListAsync();
                ViewBag.Horarios = await _context.horarios_negocio.Where(h => h.id_empresa == empresa.id_empresa).ToListAsync();
                ViewBag.Citas = await _context.citas.Where(c => c.id_empresa == empresa.id_empresa).OrderByDescending(c => c.fecha).ToListAsync();
                ViewBag.Galeria = await _context.galeria_fotos.Where(g => g.id_empresa == empresa.id_empresa).ToListAsync();
                return View(empresa);
            }

            var staff = await _context.secretarios.FirstOrDefaultAsync(s => s.id_usuario == userId);
            if (staff != null) 
            {
                var empresaStaff = await _context.empresas.FindAsync(staff.id_empresa);
                ViewBag.RolLocal = staff.subrol;
                ViewBag.Citas = await _context.citas.Where(c => c.id_empresa == staff.id_empresa).OrderByDescending(c => c.fecha).ToListAsync();
                return View(empresaStaff);
            }

            return RedirectToAction("registronegocio");
        }

        [HttpGet("mi-agenda")]
        public async Task<IActionResult> PanelSecretario()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            var staff = await _context.secretarios.FirstOrDefaultAsync(s => s.id_usuario == userId);
            
            if (staff == null) return RedirectToAction("Index");

            var empresa = await _context.empresas.FindAsync(staff.id_empresa);
            ViewBag.Citas = await _context.citas
                .Where(c => c.id_empresa == staff.id_empresa)
                .OrderBy(c => c.hora)
                .ToListAsync();

            return View("Secretario", empresa); 
        }

        [HttpGet]
        public async Task<IActionResult> Secretario()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null) return RedirectToAction("login");

            try 
            {
                var staff = await _context.secretarios
                    .Where(s => s.id_usuario == userId)
                    .FirstOrDefaultAsync();

                if (staff == null) 
                {
                    TempData["Error"] = "No tienes permisos de staff.";
                    return RedirectToAction("Index");
                }

                var empresa = await _context.empresas.FindAsync(staff.id_empresa);
                if (empresa == null) return RedirectToAction("Index");

                var citas = await _context.citas
                    .Where(c => c.id_empresa == staff.id_empresa)
                    .ToListAsync();

                ViewBag.Citas = citas ?? new List<Cita>();
                return View(empresa);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error de datos en staff: " + ex.Message);
                return RedirectToAction("Index");
            }
        }

        // =======================================================
        // GESTIÓN DE NEGOCIO (Guardar, Editar, Agregar)
        // =======================================================
        [HttpGet]
        public IActionResult registronegocio() => View();

        [HttpPost]
        public async Task<IActionResult> ProcesarRegistroNegocio(Empresa e, string rubroElegido, string rubroOtro, IFormFile? logoArchivo)
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null) return RedirectToAction("login");

            try 
            {
                e.id_administrador = userId.Value;
                e.rubro = (rubroElegido == "Otros") ? rubroOtro : rubroElegido;
                
                if (logoArchivo != null && logoArchivo.Length > 0)
                    e.logotipo_url = await GuardarFile(logoArchivo, "logos");
                
                if (string.IsNullOrEmpty(e.banner_url)) {
                    e.banner_url = "https://images.unsplash.com/photo-1497366216548-37526070297c"; 
                }
                
                e.descripcion = e.descripcion ?? "Sin descripción disponible.";
                _context.empresas.Add(e);

                var usuarioDb = await _context.usuarios.FindAsync(userId);
                if (usuarioDb != null) {
                    usuarioDb.rol = "admin";
                    _context.usuarios.Update(usuarioDb);
                    HttpContext.Session.SetString("UsuarioRol", "admin");
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Admin");
            }
            catch { return View("registronegocio"); }
        }

        [HttpPost]
        public async Task<IActionResult> EditarNegocio(Empresa e, IFormFile? logoArchivo, IFormFile? bannerArchivo)
        {
            var empresaDb = await _context.empresas.FindAsync(e.id_empresa);
            if (empresaDb == null) return NotFound();

            empresaDb.nombre_empresa = e.nombre_empresa;
            empresaDb.color_tema = e.color_tema;
            empresaDb.id_plantilla = e.id_plantilla;
            empresaDb.descripcion = e.descripcion;

            if (logoArchivo != null) empresaDb.logotipo_url = await GuardarFile(logoArchivo, "logos");
            if (bannerArchivo != null) empresaDb.banner_url = await GuardarFile(bannerArchivo, "banners");

            _context.empresas.Update(empresaDb);
            await _context.SaveChangesAsync();
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarUbicacion(int idEmpresa, double lat, double lng)
        {
            var empresa = await _context.empresas.FindAsync(idEmpresa);
            if (empresa != null) {
                empresa.ubicacion_lat = lat;
                empresa.ubicacion_lng = lng;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> SubirFotoGaleria(IFormFile archivoGaleria, int idEmpresa)
        {
            if (archivoGaleria != null && archivoGaleria.Length > 0)
            {
                string urlFoto = await GuardarFile(archivoGaleria, "galeria");
                var nuevaFoto = new GaleriaFoto { 
                    id_empresa = idEmpresa, 
                    url = urlFoto 
                };
                _context.galeria_fotos.Add(nuevaFoto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<IActionResult> BorrarFotoGaleria(int idFoto)
        {
            var foto = await _context.galeria_fotos.FindAsync(idFoto);
            if (foto != null) {
                _context.galeria_fotos.Remove(foto);
                await _context.SaveChangesAsync();
            }
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
                var nuevoStaff = new Secretario { id_usuario = usuario.id_usuario, id_empresa = idEmpresa, subrol = subrol };
                _context.secretarios.Add(nuevoStaff);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<IActionResult> GuardarHorarios(int id_empresa, IFormCollection form)
        {
            var antiguos = _context.horarios_negocio.Where(h => h.id_empresa == id_empresa);
            _context.horarios_negocio.RemoveRange(antiguos);
            await _context.SaveChangesAsync();

            for (int i = 0; i < 7; i++) {
                TimeSpan.TryParse(form[$"apertura_{i}"], out TimeSpan ap);
                TimeSpan.TryParse(form[$"cierre_{i}"], out TimeSpan ci);
                _context.horarios_negocio.Add(new HorarioNegocio {
                    id_empresa = id_empresa, dia_semana = i, hora_apertura = ap, hora_cierre = ci,
                    es_descanso = form[$"descanso_{i}"].Count > 0
                });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Admin");
        }

        // =======================================================
        // VISTAS DEL CLIENTE Y RESERVAS (CITAS)
        // =======================================================
  [HttpGet("reservar/{nombreUrl}")]
public async Task<IActionResult> PaginaCliente(string nombreUrl)
{
    var empresa = await _context.empresas
        .FirstOrDefaultAsync(e => e.nombre_empresa.Replace(" ", "-").ToLower() == nombreUrl.ToLower());
    
    if (empresa == null) return NotFound();

    ViewBag.Servicios = await _context.servicios.Where(s => s.id_empresa == empresa.id_empresa && s.activo).ToListAsync();
    
    ViewBag.Personal = await (from s in _context.secretarios
                               join us in _context.usuarios on s.id_usuario equals us.id_usuario
                               where s.id_empresa == empresa.id_empresa && s.subrol == "prestador"
                               select new { us.nombre, s.id_secretario }).ToListAsync();

    ViewBag.Resenas = await (from r in _context.resenas
                              join us in _context.usuarios on r.id_usuario equals us.id_usuario
                              where r.id_empresa == empresa.id_empresa
                              select new { r.comentario, r.calificacion, us.nombre, us.foto_url }).ToListAsync();

    ViewBag.Galeria = await _context.galeria_fotos.Where(g => g.id_empresa == empresa.id_empresa).ToListAsync();

    // NUEVO: Calculamos el total de citas para el contador (ignorando las canceladas)
    ViewBag.TotalCitas = await _context.citas.CountAsync(c => c.id_empresa == empresa.id_empresa && c.estatus != "cancelada");

    return View(empresa.id_plantilla == 2 ? "Plantilla_2" : "Plantilla_1", empresa);
}
        [HttpGet]
        public async Task<IActionResult> GetHorariosDisponibles(int idEmpresa, DateTime fecha)
        {
            var horario = await _context.horarios_negocio.FirstOrDefaultAsync(h => h.id_empresa == idEmpresa && h.dia_semana == (int)fecha.DayOfWeek);
            if (horario == null || horario.es_descanso) return Json(new { disponibles = new List<string>() });

            var horas = new List<string>();
            var act = horario.hora_apertura;
            while(act < horario.hora_cierre) {
                horas.Add(act.ToString(@"hh\:mm"));
                act = act.Add(TimeSpan.FromMinutes(30));
            }
            return Json(new { disponibles = horas });
        }

        [HttpPost]
        public async Task<IActionResult> AgendarCita(Cita nuevaCita)
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null) return Json(new { success = false, mensaje = "Sesión expirada." });

            try 
            {
                var esCliente = await _context.clientes.AnyAsync(c => c.id_usuario == userId);
                if (!esCliente)
                {
                    var nuevoCliente = new Cliente {
                        id_usuario = userId.Value,
                        numero_telefono = "0000000000" 
                    };
                    _context.clientes.Add(nuevoCliente);
                    await _context.SaveChangesAsync(); 
                }

                var expediente = await _context.expedientes.FirstOrDefaultAsync(e => e.id_cliente == userId);
                if (expediente == null)
                {
                    expediente = new Expediente {
                        id_cliente = userId.Value,
                        folio = "AXC-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                        fecha_creacion = DateTime.Now
                    };
                    _context.expedientes.Add(expediente);
                    await _context.SaveChangesAsync();
                }

                nuevaCita.id_expediente = expediente.id_expediente;
                nuevaCita.id_usuario_tramito = userId.Value;
                nuevaCita.estatus = "pendiente";
                
                _context.citas.Add(nuevaCita);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR SQL: " + ex.InnerException?.Message ?? ex.Message);
                return Json(new { success = false, mensaje = "Error de integridad: Asegúrate de que el usuario tenga un registro de Cliente." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarEstatusCita(int id_cita, string nuevoEstado)
        {
            try {
                var cita = await _context.citas.FindAsync(id_cita);
                if (cita == null) return Json(new { success = false });

                cita.estatus = nuevoEstado; 
                _context.citas.Update(cita);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch {
                return Json(new { success = false });
            }
        }

      [HttpPost]
public async Task<IActionResult> GuardarResena(int id_empresa, int calificacion, string? comentario)
{
    try 
    {
        var userId = HttpContext.Session.GetInt32("UsuarioId");
        if (userId == null) 
            return Json(new { success = false, mensaje = "Tu sesión caducó. Por favor, vuelve a iniciar sesión." });

        var nueva = new Resena {
            id_empresa = id_empresa,
            id_usuario = userId.Value,
            calificacion = calificacion,
            comentario = comentario ?? "",
            fecha = DateTime.Now
        };

        // Forzamos el guardado ignorando validaciones estrictas de campos nulos extra
        ModelState.Clear(); 
        
        _context.resenas.Add(nueva);
        await _context.SaveChangesAsync();
        
        return Json(new { success = true });
    }
    catch (Exception ex) 
    {
        // Extraemos el error real de la base de datos (Supabase)
        string errorReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        Console.WriteLine("\n\n❌ ERROR AL GUARDAR RESEÑA: " + errorReal + "\n\n");
        return Json(new { success = false, mensaje = errorReal });
    }
}
        // =======================================================
        // APIs ESTADÍSTICAS Y GRÁFICAS
        // =======================================================
        [HttpGet]
        public async Task<IActionResult> GetDatosGraficas(int idEmpresa)
        {
            var citas = await _context.citas.Where(c => c.id_empresa == idEmpresa).ToListAsync();
            var citasPorDia = new int[7]; 
            foreach (var c in citas)
            {
                int diaIndex = ((int)c.fecha.DayOfWeek + 6) % 7;
                citasPorDia[diaIndex]++;
            }

            int canceladas = citas.Count(c => c.estatus.ToLower() == "cancelada");
            int exitosas = citas.Count - canceladas;

            return Json(new { 
                citasPorDia = citasPorDia, 
                proporcion = new int[] { exitosas, canceladas } 
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetStatsAdmin(int idEmpresa)
        {
            var todasLasCitas = await _context.citas.Where(c => c.id_empresa == idEmpresa).ToListAsync();
            return Json(new { total = todasLasCitas.Count });
        }

        // =======================================================
        // UTILERÍAS
        // =======================================================
        public async Task<IActionResult> Logout() {
            HttpContext.Session.Clear();
            // ¡Corregido! Restauramos el limpiado de cookies para Google
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

    // ---> CLASES RESTAURADAS <---
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