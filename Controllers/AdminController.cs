using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using axcan.Models;
using axcan.Data;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace axcan.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int adminId = HttpContext.Session.GetInt32("UsuarioId") ?? 1;

            var empresa = await _context.empresas.FirstOrDefaultAsync(e => e.id_administrador == adminId);
            if (empresa == null) return RedirectToAction("registronegocio", "Home");

            int empresaId = empresa.id_empresa;

            ViewBag.CitasTotales = await _context.citas.CountAsync(c => c.id_empresa == empresaId);
            ViewBag.Canceladas = await _context.citas.CountAsync(c => c.id_empresa == empresaId && c.estatus == "cancelada");
            
            ViewBag.ProximasCitas = await _context.citas
                .Where(c => c.id_empresa == empresaId && c.estatus != "cancelada")
                .OrderByDescending(c => c.fecha)
                .Take(5)
                .ToListAsync();

            return View(empresa);
        }

        [HttpPost]
        public async Task<IActionResult> CrearServicio(Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                bool existeServicio = await _context.servicios
                    .AnyAsync(s => s.nombre_servicio.ToLower() == servicio.nombre_servicio.ToLower() && s.id_empresa == servicio.id_empresa);

                if (existeServicio)
                {
                    TempData["Error"] = "¡Error! Este servicio ya existe.";
                    return RedirectToAction("Admin", "Home"); 
                }

                _context.servicios.Add(servicio);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Servicio creado correctamente.";
                return RedirectToAction("Admin", "Home");
            }
            return View(servicio);
        }

        [HttpGet]
        public async Task<IActionResult> EditarServicio(int id)
        {
            var servicio = await _context.servicios.FindAsync(id);
            if (servicio == null) return NotFound();
            return View(servicio);
        }

        [HttpPost]
        public async Task<IActionResult> EditarServicio(Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.servicios.Update(servicio);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Servicio actualizado.";
                    return RedirectToAction("Admin", "Home");
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["Error"] = "Error al actualizar.";
                    return View(servicio);
                }
            }
            return View(servicio);
        }
    }
}