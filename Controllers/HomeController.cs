
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
//procesar registro 
[HttpPost]
public async Task<IActionResult> ProcesarRegistro(Usuario u)
{
    try 
    {
        // 1. Validamos que no exista ni el username ni el correo
        var existeUser = await _context.usuarios.AnyAsync(x => x.username == u.username);
        var existeCorreo = await _context.usuarios.AnyAsync(x => x.correo == u.correo);

        if (existeUser) {
            ViewBag.Error = "Ese nombre de usuario ya está ocupado, padrino. Intenta otro.";
            return View("registro");
        }
        if (existeCorreo) {
            ViewBag.Error = "Ese correo ya está registrado. Mejor inicia sesión.";
            return View("registro");
        }

        // 2. Le ponemos sus valores por defecto
        u.rol = "usuario"; 
        u.fecha_registro = DateTime.Now;
        
        // 3. ¡Lo guardamos en Supabase!
        _context.usuarios.Add(u);
        await _context.SaveChangesAsync();
        
        // 4. Mensaje de éxito y lo mandamos al Login
        TempData["Mensaje"] = "¡Bienvenido a AXCAN! Tu cuenta ha sido creada, ya puedes iniciar sesión.";
       return RedirectToAction("RegistroExitoso");
    }
    catch (Exception ex) 
    {
        // Si la base de datos explota, te escupe el error exacto aquí
        ViewBag.Error = "Error interno al registrar: " + (ex.InnerException?.Message ?? ex.Message);
        return View("registro");
    }
}
        // =======================================================
        // EL BUSCADOR Y LAS ESTRELLAS
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
    
    // USAMOS LA CLASE NUEVA EN LUGAR DE DYNAMIC
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
        // PANEL ADMIN SPA PREMIUM
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
        // PLANTILLAS DINÁMICAS (Para Axel)
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
// datos para graficas 
[HttpGet]
public async Task<IActionResult> GetStatsAdmin(int idEmpresa)
{
    var inicioSemana = DateTime.Now.Date.AddDays(-7);
    
    // Usamos 'fecha' en lugar de 'fecha_cita'
    var todasLasCitas = await _context.citas
        .Where(c => c.id_empresa == idEmpresa)
        .ToListAsync();

    var citasPorDia = todasLasCitas
        .Where(c => c.fecha >= inicioSemana)
        .GroupBy(c => c.fecha.ToString("dddd"))
        .Select(g => new { Dia = g.Key, Total = g.Count() })
        .ToList();

    var hechas = todasLasCitas.Count(c => 
        string.Equals(c.estatus, "Confirmada", StringComparison.OrdinalIgnoreCase) || 
        string.Equals(c.estatus, "Completada", StringComparison.OrdinalIgnoreCase));

    var canceladas = todasLasCitas.Count(c => 
        string.Equals(c.estatus, "Cancelada", StringComparison.OrdinalIgnoreCase));

    var diaMasFuerte = todasLasCitas
        .GroupBy(c => c.fecha.DayOfWeek)
        .Select(g => new { DiaSemana = g.Key.ToString(), Cantidad = g.Count() })
        .OrderByDescending(x => x.Cantidad)
        .ToList();

    return Json(new {
        barrasCitasDia = citasPorDia,
        pastelCancelaciones = new { hechas, canceladas },
        barrasDiaFuerte = diaMasFuerte
    });
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

    // Cambiamos 'fecha_cita' por 'fecha' y 'hora_cita' por 'hora'
    var citasOcupadas = await _context.citas
        .Where(c => c.id_empresa == idEmpresa && c.fecha.Date == fecha.Date)
        .Select(c => c.hora) 
        .ToListAsync();

    var horasDisponibles = new List<string>();
    var horaActual = horarioDia.hora_apertura;

    while (horaActual < horarioDia.hora_cierre)
    {
        // Formateamos el TimeSpan a string para el JS de Axel
        string horaFormateada = horaActual.ToString(@"hh\:mm");
        
        // Comparamos el TimeSpan actual con la lista de horas ocupadas
        if (!citasOcupadas.Any(h => h.ToString(@"hh\:mm") == horaFormateada))
        {
            horasDisponibles.Add(horaFormateada);
        }
        horaActual = horaActual.Add(TimeSpan.FromMinutes(30));
    }

    return Json(new { disponibles = horasDisponibles });
}
//agendar cita
        [HttpPost]
public async Task<IActionResult> AgendarCita(Cita nuevaCita)
{
    // Eliminamos 'fecha_creacion' porque no existe en tu tabla SQL
    nuevaCita.estatus = "Confirmada";
    
    // Asegúrate que desde el JS Axel mande:
    // id_expediente e id_usuario_tramito porque son NOT NULL
    
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
        // AUTENTICACIÓN Y PERFIL DE USUARIO
        // =======================================================
        public IActionResult login() => View();
        public IActionResult RegistroExitoso() => View();
        public IActionResult registro() => View();
        public IActionResult registronegocio() => View();

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
    }
