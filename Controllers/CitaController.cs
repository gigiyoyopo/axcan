using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using axcan.Models;
using axcan.Data; 
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace axcan.Controllers
{
    public class CitaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CitaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> MisCitas()
        {
            int usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;

            if (usuarioId == 0) return RedirectToAction("login", "Home");

            var misCitas = await _context.citas
                .Where(c => c.id_usuario_tramito == usuarioId)
                .OrderByDescending(c => c.fecha)
                .ToListAsync();

            return View(misCitas);
        }

        [HttpPost]
        public async Task<IActionResult> Agendar(Cita cita)
        {
            int usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;
            if (usuarioId == 0) return RedirectToAction("login", "Home");

            if (ModelState.IsValid)
            {
                cita.id_usuario_tramito = usuarioId;
                cita.estatus = "pendiente"; 

                try 
                {
                    _context.citas.Add(cita);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Cita agendada correctamente.";
                    return RedirectToAction("MisCitas");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al guardar: " + ex.Message;
                    return RedirectToAction("MisCitas");
                }
            }
            
            TempData["Error"] = "Datos inválidos.";
            return RedirectToAction("MisCitas");
        }

        [HttpPost]
        public async Task<IActionResult> Cancelar(int id_cita)
        {
            var cita = await _context.citas.FindAsync(id_cita);

            if (cita == null)
            {
                TempData["Error"] = "La cita no existe.";
                return RedirectToAction("MisCitas");
            }

            int usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;
            
            if (cita.id_usuario_tramito != usuarioId) 
            {
                TempData["Error"] = "No tienes permiso.";
                return RedirectToAction("MisCitas");
            }

            cita.estatus = "cancelada";
            
            try 
            {
                _context.citas.Update(cita);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Cita cancelada.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cancelar: " + ex.Message;
            }

            return RedirectToAction("MisCitas");
        }
    }
}