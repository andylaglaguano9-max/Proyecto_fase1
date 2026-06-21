using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaApp.Data;
using VeterinariaApp.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace VeterinariaApp.Controllers
{
    public class EspeciesController : Controller
    {
        private readonly AppDbContext _context;

        public EspeciesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string orden, string buscar, int pagina = 1)
        {
            int registrosPorPagina = 50;
            var query = _context.Especies.Where(e => e.Activo).AsQueryable();

            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(e => e.Nombre.Contains(buscar));
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

            var especies = await query
                
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.Buscar = buscar;

            return View(especies);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Especie especie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(especie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(especie);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var especie = await _context.Especies.FindAsync(id);
            if (especie == null || !especie.Activo) return NotFound();
            
            return View(especie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Especie especie)
        {
            if (id != especie.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var original = await _context.Especies.FindAsync(id);
                if (original != null)
                {
                    original.Nombre = especie.Nombre;
                    original.FechaActualizacion = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(especie);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var especie = await _context.Especies.FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (especie == null) return NotFound();

            return View(especie);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var especie = await _context.Especies.FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (especie == null) return NotFound();

            return View(especie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var especie = await _context.Especies.FindAsync(id);
            if (especie != null)
            {
                especie.Activo = false;
                especie.FechaEliminacion = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
