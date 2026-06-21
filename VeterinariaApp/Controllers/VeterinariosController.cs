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
    public class VeterinariosController : Controller
    {
        private readonly AppDbContext _context;

        public VeterinariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Veterinarios
        public async Task<IActionResult> Index(string orden, string buscar = "", int pagina = 1)
        {
            int pageSize = 20;
            var query = _context.Veterinarios
                .Include(v => v.Sucursal)
                .AsNoTracking()
                .Where(v => v.Activo);

            if (!string.IsNullOrEmpty(buscar))
                query = query.Where(v => v.Nombre.Contains(buscar) ||
                    v.Apellido.Contains(buscar) ||
                    v.Especialidad.Contains(buscar));

            
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

        // GET: Veterinarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var veterinario = await _context.Veterinarios
                .Include(v => v.Sucursal)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (veterinario == null) return NotFound();

            return View(veterinario);
        }

        // GET: Veterinarios/Create
        public IActionResult Create()
        {
            ViewData["SucursalId"] = new SelectList(_context.Sucursales.Where(s => s.Activo).Take(50), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Apellido,Especialidad,Telefono,SucursalId")] Veterinario veterinario)
        {
            if (ModelState.IsValid)
            {
                veterinario.FechaCreacion = DateTime.UtcNow;
                veterinario.Activo = true;
                _context.Add(veterinario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SucursalId"] = new SelectList(_context.Sucursales.Where(s => s.Activo).Take(50), "Id", "Nombre", veterinario.SucursalId);
            return View(veterinario);
        }

        // GET: Veterinarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var veterinario = await _context.Veterinarios.FindAsync(id);
            if (veterinario == null || !veterinario.Activo) return NotFound();
            
            ViewData["SucursalId"] = new SelectList(_context.Sucursales.Where(s => s.Activo).Take(50), "Id", "Nombre", veterinario.SucursalId);
            return View(veterinario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Especialidad,Telefono,SucursalId")] Veterinario veterinario)
        {
            if (id != veterinario.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Veterinarios.FindAsync(id);
                    if (existing == null || !existing.Activo) return NotFound();

                    existing.Nombre = veterinario.Nombre;
                    existing.Apellido = veterinario.Apellido;
                    existing.Especialidad = veterinario.Especialidad;
                    existing.Telefono = veterinario.Telefono;
                    existing.SucursalId = veterinario.SucursalId;
                    existing.FechaActualizacion = DateTime.UtcNow;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VeterinarioExists(veterinario.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SucursalId"] = new SelectList(_context.Sucursales.Where(s => s.Activo).Take(50), "Id", "Nombre", veterinario.SucursalId);
            return View(veterinario);
        }

        // GET: Veterinarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var veterinario = await _context.Veterinarios
                .Include(v => v.Sucursal)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (veterinario == null) return NotFound();

            return View(veterinario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var veterinario = await _context.Veterinarios.FindAsync(id);
            if (veterinario != null)
            {
                veterinario.Activo = false;
                veterinario.FechaEliminacion = DateTime.UtcNow;
                _context.Update(veterinario);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VeterinarioExists(int id)
        {
            return _context.Veterinarios.Any(e => e.Id == id && e.Activo);
        }
    }
}
