using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VeterinariaApp.Data;
using VeterinariaApp.Models;
using VeterinariaApp.Filters;
using Microsoft.AspNetCore.Http;

namespace VeterinariaApp.Controllers
{
    [SessionAuthorize]
    public class MascotasController : Controller
    {
        private readonly AppDbContext _context;

        public MascotasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Mascotas
        public async Task<IActionResult> Index(string orden, string buscar = "", int pagina = 1)
        {
            int pageSize = 20;
            var query = _context.Mascotas
                .Include(m => m.Dueno)
                .Include(m => m.Especie)
                // Desactiva el seguimiento de cambios en EF Core para operaciones de solo lectura,
                // mejorando el rendimiento y reduciendo significativamente el consumo de memoria.
                .AsNoTracking()
                .Where(m => m.Activo);

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentRol = HttpContext.Session.GetString("Rol");

            if (currentRol == "Usuario")
            {
                query = query.Where(m => m.Dueno != null && m.Dueno.UsuarioId == currentUserId);
            }
            // Los Doctores pueden ver todas las mascotas (sus pacientes), o podríamos limitarlo
            // a las mascotas que tienen citas con ellos. Por simplicidad, dejamos que vean todas
            // o solo filtramos a Usuario.

            if (!string.IsNullOrEmpty(buscar))
                query = query.Where(m => m.Nombre.Contains(buscar) ||
                    (m.Dueno != null && m.Dueno.Nombre.Contains(buscar)) ||
                    (m.Especie != null && m.Especie.Nombre.Contains(buscar)));

            
            // USO DE LINQ (Language Integrated Query): Ordenamiento en tiempo de ejecución.
            // Se invoca al método '.OrderByDescending()' pasándole una expresión Lambda.
            // Entity Framework traduce este método LINQ a un 'ORDER BY' de SQL optimizado en el motor.
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
                // Implementación de paginación a nivel de base de datos (OFFSET/LIMIT)
                // para evitar cargar la totalidad de registros en memoria y optimizar el tiempo de respuesta.
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / pageSize);
            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.Buscar = buscar;

            return View(datos);
        }

        // GET: Mascotas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var mascota = await _context.Mascotas
                .Include(m => m.Dueno)
                .Include(m => m.Especie)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (mascota == null) return NotFound();

            return View(mascota);
        }

        // GET: Mascotas/Create
        public IActionResult Create()
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentRol = HttpContext.Session.GetString("Rol");
            var duenosQuery = _context.Duenos.Where(d => d.Activo);
            if (currentRol == "Usuario") { duenosQuery = duenosQuery.Where(d => d.UsuarioId == currentUserId); }
            var duenos = duenosQuery.Take(50).Select(d => new { d.Id, NombreCompleto = d.Nombre + " " + d.Apellido }).ToList();
            
            ViewData["DuenoId"] = new SelectList(duenos, "Id", "NombreCompleto");
            ViewData["EspecieId"] = new SelectList(_context.Especies.Where(e => e.Activo).Take(50), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Nombre,FechaNacimiento,Peso,DuenoId,EspecieId")] Mascota mascota)
        {
            if (ModelState.IsValid)
            {
                mascota.FechaCreacion = DateTime.UtcNow;
                mascota.Activo = true;
                _context.Add(mascota);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var currentUserId = HttpContext.Session.GetInt32("UserId"); var currentRol = HttpContext.Session.GetString("Rol"); var duenosQuery = _context.Duenos.Where(d => d.Activo); if (currentRol == "Usuario") { duenosQuery = duenosQuery.Where(d => d.UsuarioId == currentUserId); } var duenos = duenosQuery.Take(50).Select(d => new { d.Id, NombreCompleto = d.Nombre + " " + d.Apellido }).ToList();
            ViewData["DuenoId"] = new SelectList(duenos, "Id", "NombreCompleto", mascota.DuenoId);
            ViewData["EspecieId"] = new SelectList(_context.Especies.Where(e => e.Activo).Take(50), "Id", "Nombre", mascota.EspecieId);
            return View(mascota);
        }

        // GET: Mascotas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota == null || !mascota.Activo) return NotFound();
            
            var currentUserId = HttpContext.Session.GetInt32("UserId"); var currentRol = HttpContext.Session.GetString("Rol"); var duenosQuery = _context.Duenos.Where(d => d.Activo); if (currentRol == "Usuario") { duenosQuery = duenosQuery.Where(d => d.UsuarioId == currentUserId); } var duenos = duenosQuery.Take(50).Select(d => new { d.Id, NombreCompleto = d.Nombre + " " + d.Apellido }).ToList();
            ViewData["DuenoId"] = new SelectList(duenos, "Id", "NombreCompleto", mascota.DuenoId);
            ViewData["EspecieId"] = new SelectList(_context.Especies.Where(e => e.Activo).Take(50), "Id", "Nombre", mascota.EspecieId);
            return View(mascota);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,FechaNacimiento,Peso,DuenoId,EspecieId")] Mascota mascota)
        {
            if (id != mascota.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Mascotas.FindAsync(id);
                    if (existing == null || !existing.Activo) return NotFound();

                    existing.Nombre = mascota.Nombre;
                    existing.FechaNacimiento = mascota.FechaNacimiento;
                    existing.Peso = mascota.Peso;
                    existing.DuenoId = mascota.DuenoId;
                    existing.EspecieId = mascota.EspecieId;
                    existing.FechaActualizacion = DateTime.UtcNow;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MascotaExists(mascota.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            var currentUserId = HttpContext.Session.GetInt32("UserId"); var currentRol = HttpContext.Session.GetString("Rol"); var duenosQuery = _context.Duenos.Where(d => d.Activo); if (currentRol == "Usuario") { duenosQuery = duenosQuery.Where(d => d.UsuarioId == currentUserId); } var duenos = duenosQuery.Take(50).Select(d => new { d.Id, NombreCompleto = d.Nombre + " " + d.Apellido }).ToList();
            ViewData["DuenoId"] = new SelectList(duenos, "Id", "NombreCompleto", mascota.DuenoId);
            ViewData["EspecieId"] = new SelectList(_context.Especies.Where(e => e.Activo).Take(50), "Id", "Nombre", mascota.EspecieId);
            return View(mascota);
        }

        // GET: Mascotas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var mascota = await _context.Mascotas
                .Include(m => m.Dueno)
                .Include(m => m.Especie)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (mascota == null) return NotFound();

            return View(mascota);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota != null)
            {
                mascota.Activo = false;
                mascota.FechaEliminacion = DateTime.UtcNow;
                _context.Update(mascota);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MascotaExists(int id)
        {
            return _context.Mascotas.Any(e => e.Id == id && e.Activo);
        }
    }
}
