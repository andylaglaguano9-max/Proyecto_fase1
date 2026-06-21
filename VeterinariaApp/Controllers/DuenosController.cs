using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using VeterinariaApp.Data;
using VeterinariaApp.Models;
using VeterinariaApp.Filters;

namespace VeterinariaApp.Controllers
{
    [SessionAuthorize]
    public class DuenosController : Controller
    {
        private readonly AppDbContext _context;

        public DuenosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Duenos
        public async Task<IActionResult> Index(string orden, string buscar = "", int pagina = 1)
        {
            var currentRol = HttpContext.Session.GetString("Rol");
            if (currentRol == "Usuario") return Unauthorized("No tienes permiso para ver esta sección.");

            int pageSize = 20;
            var query = _context.Duenos.AsNoTracking().Where(d => d.Activo);

            if (!string.IsNullOrEmpty(buscar))
                query = query.Where(d => d.Nombre.Contains(buscar) || d.Apellido.Contains(buscar) || d.Correo.Contains(buscar));

            
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

        // GET: Duenos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var dueno = await _context.Duenos
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (dueno == null) return NotFound();

            return View(dueno);
        }

        // GET: Duenos/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Apellido,Telefono,Direccion,Correo")] Dueno dueno)
        {
            if (ModelState.IsValid)
            {
                dueno.FechaCreacion = DateTime.UtcNow;
                dueno.Activo = true;
                _context.Add(dueno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dueno);
        }

        // GET: Duenos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var dueno = await _context.Duenos.FindAsync(id);
            if (dueno == null || !dueno.Activo) return NotFound();
            
            return View(dueno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Telefono,Direccion,Correo")] Dueno dueno)
        {
            if (id != dueno.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Duenos.FindAsync(id);
                    if (existing == null || !existing.Activo) return NotFound();

                    existing.Nombre = dueno.Nombre;
                    existing.Apellido = dueno.Apellido;
                    existing.Telefono = dueno.Telefono;
                    existing.Direccion = dueno.Direccion;
                    existing.Correo = dueno.Correo;
                    existing.FechaActualizacion = DateTime.UtcNow;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DuenoExists(dueno.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dueno);
        }

        // GET: Duenos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var dueno = await _context.Duenos
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (dueno == null) return NotFound();

            return View(dueno);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dueno = await _context.Duenos.FindAsync(id);
            if (dueno != null)
            {
                dueno.Activo = false;
                dueno.FechaEliminacion = DateTime.UtcNow;
                _context.Update(dueno);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DuenoExists(int id)
        {
            return _context.Duenos.Any(e => e.Id == id && e.Activo);
        }
    }
}
