using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaApp.Data;
using VeterinariaApp.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace VeterinariaApp.Controllers
{
    public class VentasController : Controller
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string orden, string buscar, int pagina = 1)
        {
            int registrosPorPagina = 50;
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentRol = HttpContext.Session.GetString("Rol");
            var query = _context.Ventas
                .Include(v => v.Dueno)
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .Where(e => e.Activo).AsQueryable();
            
            if (currentRol == "Usuario") 
            {
                query = query.Where(v => v.Dueno.UsuarioId == currentUserId);
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

            var list = await query
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.Buscar = buscar;

            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venta entity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var entity = await _context.Ventas.FindAsync(id);
            if (entity == null || !entity.Activo) return NotFound();
            
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venta entity)
        {
            if (id != entity.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var original = await _context.Ventas.FindAsync(id);
                if (original != null)
                {
                    _context.Entry(original).CurrentValues.SetValues(entity);
                    original.FechaActualizacion = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var entity = await _context.Ventas.FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (entity == null) return NotFound();

            return View(entity);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var entity = await _context.Ventas.FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (entity == null) return NotFound();

            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.Ventas.FindAsync(id);
            if (entity != null)
            {
                entity.Activo = false;
                entity.FechaEliminacion = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
