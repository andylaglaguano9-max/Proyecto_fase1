using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VeterinariaApp.Data;
using VeterinariaApp.Models;
using VeterinariaApp.Filters;

namespace VeterinariaApp.Controllers
{
    [SessionAuthorize]
    public class SucursalesController : Controller
    {
        private readonly AppDbContext _context;

        public SucursalesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Sucursales
        public async Task<IActionResult> Index(string orden, string buscar = "", int pagina = 1)
        {
            int pageSize = 20;
            var query = _context.Sucursales
                .Include(s => s.Ciudad)
                .AsNoTracking()
                .Where(s => s.Activo);

            if (!string.IsNullOrEmpty(buscar))
                query = query.Where(s => s.Nombre.Contains(buscar) ||
                    (s.Ciudad != null && s.Ciudad.Nombre.Contains(buscar)));

            
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
            var datos = await query
                
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / pageSize);
            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.Buscar = buscar;

            return View(datos);
        }

        // GET: Sucursales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sucursal = await _context.Sucursales
                .Include(s => s.Ciudad)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (sucursal == null) return NotFound();

            return View(sucursal);
        }

        // GET: Sucursales/Create
        public IActionResult Create()
        {
            var ciudades = _context.Ciudades.Where(c => c.Activo).Take(50).ToList();
            ViewData["CiudadId"] = new SelectList(ciudades, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Direccion,CiudadId")] Sucursal sucursal)
        {
            if (ModelState.IsValid)
            {
                sucursal.FechaCreacion = DateTime.UtcNow;
                sucursal.Activo = true;
                _context.Add(sucursal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CiudadId"] = new SelectList(_context.Ciudades.Where(c => c.Activo).Take(50), "Id", "Nombre", sucursal.CiudadId);
            return View(sucursal);
        }

        // GET: Sucursales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sucursal = await _context.Sucursales.FindAsync(id);
            if (sucursal == null || !sucursal.Activo) return NotFound();
            
            ViewData["CiudadId"] = new SelectList(_context.Ciudades.Where(c => c.Activo).Take(50), "Id", "Nombre", sucursal.CiudadId);
            return View(sucursal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Direccion,CiudadId")] Sucursal sucursal)
        {
            if (id != sucursal.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Sucursales.FindAsync(id);
                    if (existing == null || !existing.Activo) return NotFound();

                    existing.Nombre = sucursal.Nombre;
                    existing.Direccion = sucursal.Direccion;
                    existing.CiudadId = sucursal.CiudadId;
                    existing.FechaActualizacion = DateTime.UtcNow;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SucursalExists(sucursal.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CiudadId"] = new SelectList(_context.Ciudades.Where(c => c.Activo).Take(50), "Id", "Nombre", sucursal.CiudadId);
            return View(sucursal);
        }

        // GET: Sucursales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var sucursal = await _context.Sucursales
                .Include(s => s.Ciudad)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (sucursal == null) return NotFound();

            return View(sucursal);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sucursal = await _context.Sucursales.FindAsync(id);
            if (sucursal != null)
            {
                sucursal.Activo = false;
                sucursal.FechaEliminacion = DateTime.UtcNow;
                _context.Update(sucursal);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SucursalExists(int id)
        {
            return _context.Sucursales.Any(e => e.Id == id && e.Activo);
        }
    }
}
