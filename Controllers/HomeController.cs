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

        // =======================================================
        // FASE 4: VISTAS PÚBLICAS Y ENRUTAMIENTO DINÁMICO (URLs)
        // =======================================================
        
        public async Task<IActionResult> Index() 
        {
            var empresas = await _context.empresas.ToListAsync();
            return View(empresas);
        }

        // ¡LA MAGIA DE LAS PÁGINAS INDEPENDIENTES! (axcan.com/reservar/barberia-paco)
        [Route("reservar/{nombreUrl}")]
        public async Task<IActionResult> PaginaCliente(string nombreUrl)
        {
            var empresa = await _context.empresas
                .FirstOrDefaultAsync(e => e.nombre_empresa.Replace(" ", "-").ToLower() == nombreUrl.ToLower());

            if (empresa == null) return NotFound("Página no encontrada.");
            
            // Aquí le mandamos los datos al molde de Axel
            return View("PlantillaReserva", empresa); 
        }

        public IActionResult login() => View();
        public IActionResult registro() => View();
        public IActionResult registronegocio() => View();

        // =======================================================
        // FASE 2: LÓGICA DE ROLES (Admin vs Secretario)
        // =======================================================
        
        [Route("admin")]
public async Task<IActionResult> Admin()
{
    var userId = HttpContext.Session.GetInt32("UsuarioId");
    if (userId == null) return RedirectToAction("login");

    // 1. SI ES EL DUEÑO (ADMINISTRADOR)
    var empresa = await _context.empresas.FirstOrDefaultAsync(e => e.id_administrador == userId);
    
    if (empresa != null) {
        ViewBag.RolLocal = "administrador";
        
        // --- MAGIA: Traemos los Servicios Reales ---
        ViewBag.Servicios = await _context.servicios
            .Where(s => s.id_empresa == empresa.id_empresa).ToListAsync();

        // --- MAGIA: Traemos el Personal Real (Join con Usuarios) ---
        var personal = await (from s in _context.secretarios
                              join u in _context.usuarios on s.id_usuario equals u.id_usuario
                              where s.id_empresa == empresa.id_empresa
                              select new { 
                                  u.nombre, 
                                  u.username, 
                                  s.subrol,
                                  s.id_secretario
                              }).ToListAsync();
        ViewBag.Personal = personal;

        return View(empresa);
    }

    // 2. SI ES EL EMPLEADO (SECRETARIO / PRESTADOR)
    var empleado = await _context.secretarios.FirstOrDefaultAsync(s => s.id_usuario == userId);
    if (empleado != null) {
        var empresaEmpleado = await _context.empresas.FindAsync(empleado.id_empresa);
        ViewBag.RolLocal = empleado.subrol;
        
        // --- MAGIA: Traemos las Citas Reales del negocio ---
        ViewBag.Citas = await _context.citas
            .Where(c => c.id_empresa == empleado.id_empresa)
            .ToListAsync();

        return View(empresaEmpleado);
    }

    return RedirectToAction("registronegocio");
}

        // Lógica de Login (Igual que antes)
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

        // =======================================================
        // FASE 1: GESTIÓN DE NEGOCIO (Servicios, Personal y Horarios)
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

        // =======================================================
        // FASE 3: EL MOTOR DEL CALENDARIO DE AXEL
        // =======================================================

        [HttpPost]
        public async Task<IActionResult> AgendarCita(Cita nuevaCita)
        {
            nuevaCita.estatus = "pendiente";
            _context.citas.Add(nuevaCita);
            await _context.SaveChangesAsync();
            
            // Retorna a la página de éxito o recarga la plantilla
            return RedirectToAction("Index"); 
        }

        [HttpGet]
        public async Task<JsonResult> GetDatosCalendario(int idEmpresa)
        {
            // Este endpoint es para que el JS de Axel cargue los selects dinámicamente
            var servicios = await _context.servicios.Where(s => s.id_empresa == idEmpresa && s.activo).ToListAsync();
            var personal = await _context.secretarios.Where(s => s.id_empresa == idEmpresa && s.subrol == "prestador").ToListAsync();
            
            return Json(new { servicios = servicios, prestadores = personal });
        }

        // =======================================================
        // MÉTODOS AUXILIARES
        // =======================================================

        public IActionResult Logout() {
            HttpContext.Session.Clear();
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