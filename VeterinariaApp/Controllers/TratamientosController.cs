using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VeterinariaApp.Data;
using VeterinariaApp.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace VeterinariaApp.Controllers
{
    public class TratamientosController : Controller
    {
        private readonly AppDbContext _context;

        public TratamientosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string orden, string buscar, int pagina = 1)
        {
            int registrosPorPagina = 50;
            var query = _context.Tratamientos.Include(t => t.Cita).Where(t => t.Activo).AsQueryable();

            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(t => t.Descripcion.Contains(buscar));
            }

            
            // ORDENAMIENTO LOGIC
            ViewBag.OrdenActual = orden;
            bool hasNombre = query.ElementType.GetProperty("Nombre") != null;

            switch (orden)
            {
                case "recientes":
                    query = query.OrderByDescending(x => x.FechaCreacion);
                    break;
                case "antiguos":
                    query = query.OrderBy(x => x.FechaCreacion);
                    break;
                case "az":
                    if (hasNombre) query = query.OrderBy(x => Microsoft.EntityFrameworkCore.EF.Property<string>(x, "Nombre"));
                    else query = query.OrderBy(x => x.Id);
                    break;
                case "za":
                    if (hasNombre) query = query.OrderByDescending(x => Microsoft.EntityFrameworkCore.EF.Property<string>(x, "Nombre"));
                    else query = query.OrderByDescending(x => x.Id);
                    break;
                default:
                    query = query.OrderByDescending(x => x.FechaCreacion);
                    break;
            }

            int totalRegistros = await query.CountAsync();
            int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)registrosPorPagina);

            var tratamientos = await query
                .OrderByDescending(t => t.FechaCreacion)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.Buscar = buscar;

            return View(tratamientos);
        }

        public IActionResult Create()
        {
            ViewData["CitaId"] = new SelectList(_context.Citas.Where(c => c.Activo).Take(50), "Id", "Motivo");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tratamiento tratamiento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tratamiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CitaId"] = new SelectList(_context.Citas.Where(c => c.Activo).Take(50), "Id", "Motivo", tratamiento.CitaId);
            return View(tratamiento);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tratamiento = await _context.Tratamientos.FindAsync(id);
            if (tratamiento == null || !tratamiento.Activo) return NotFound();
            
            ViewData["CitaId"] = new SelectList(_context.Citas.Where(c => c.Activo).Take(50), "Id", "Motivo", tratamiento.CitaId);
            return View(tratamiento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tratamiento tratamiento)
        {
            if (id != tratamiento.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var original = await _context.Tratamientos.FindAsync(id);
                if (original != null)
                {
                    original.CitaId = tratamiento.CitaId;
                    original.Descripcion = tratamiento.Descripcion;
                    original.Costo = tratamiento.Costo;
                    original.FechaActualizacion = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CitaId"] = new SelectList(_context.Citas.Where(c => c.Activo).Take(50), "Id", "Motivo", tratamiento.CitaId);
            return View(tratamiento);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tratamiento = await _context.Tratamientos
                .Include(t => t.Cita)
                .FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (tratamiento == null) return NotFound();

            return View(tratamiento);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tratamiento = await _context.Tratamientos
                .Include(t => t.Cita)
                .FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (tratamiento == null) return NotFound();

            return View(tratamiento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tratamiento = await _context.Tratamientos.FindAsync(id);
            if (tratamiento != null)
            {
                tratamiento.Activo = false;
                tratamiento.FechaEliminacion = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
