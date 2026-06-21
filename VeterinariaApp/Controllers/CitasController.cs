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
    public class CitasController : Controller
    {
        private readonly AppDbContext _context;

        public CitasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Citas
        public async Task<IActionResult> Index(string orden, string buscar = "", int pagina = 1)
        {
            int pageSize = 20;
            var query = _context.Citas
                .Include(c => c.Mascota)
                .ThenInclude(m => m.Dueno)
                .Include(c => c.Veterinario)
                .Include(c => c.Sucursal)
                .AsNoTracking()
                .Where(c => c.Activo);

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentRol = HttpContext.Session.GetString("Rol");

            if (currentRol == "Usuario")
            {
                // Implementación de Data Isolation (RBAC): Filtra las citas para que los clientes
                // solo tengan acceso a los registros asociados explícitamente a su identificador de usuario.
                query = query.Where(c => c.Mascota != null && c.Mascota.Dueno != null && c.Mascota.Dueno.UsuarioId == currentUserId);
            }
            else if (currentRol == "Doctor")
            {
                // Restricción de acceso para personal médico: Limita la visualización del historial
                // exclusivamente a las citas que le han sido asignadas a este profesional.
                query = query.Where(c => c.Veterinario != null && c.Veterinario.UsuarioId == currentUserId);
            }

            // USO DE LINQ (Language Integrated Query): Filtrado dinámico de datos.
            // Se utilizan expresiones Lambda dentro del método extensor de LINQ '.Where()' para construir
            // sentencias condicionales complejas directamente sobre el árbol de expresiones, sin escribir SQL puro.
            if (!string.IsNullOrEmpty(buscar))
                query = query.Where(c => c.Motivo.Contains(buscar) ||
                    (c.Mascota != null && c.Mascota.Nombre.Contains(buscar)) ||
                    (c.Veterinario != null && c.Veterinario.Nombre.Contains(buscar)));

            
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
                .OrderByDescending(c => c.FechaCita)
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / pageSize);
            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.Buscar = buscar;

            return View(datos);
        }

        // GET: Citas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cita = await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.Veterinario)
                .Include(c => c.Sucursal)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (cita == null) return NotFound();

            return View(cita);
        }

        // GET: Citas/Create
        public IActionResult Create()
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId"); var currentRol = HttpContext.Session.GetString("Rol"); var mascotasQuery = _context.Mascotas.Where(m => m.Activo); if(currentRol == "Usuario") { mascotasQuery = mascotasQuery.Where(m => m.Dueno.UsuarioId == currentUserId); } ViewData["MascotaId"] = new SelectList(mascotasQuery.Take(50), "Id", "Nombre");
            var veterinarios = _context.Veterinarios.Where(v => v.Activo).Take(50).Select(v => new { v.Id, NombreCompleto = v.Nombre + " " + v.Apellido }).ToList();
            ViewData["VeterinarioId"] = new SelectList(veterinarios, "Id", "NombreCompleto");
            ViewData["SucursalId"] = new SelectList(_context.Sucursales.Where(s => s.Activo).Take(50), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FechaCita,Motivo,Estado,MascotaId,VeterinarioId,SucursalId")] Cita cita)
        {
            if (ModelState.IsValid)
            {
                cita.FechaCreacion = DateTime.UtcNow;
                cita.Activo = true;
                _context.Add(cita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var currentUserId = HttpContext.Session.GetInt32("UserId"); var currentRol = HttpContext.Session.GetString("Rol"); var mascotasQuery = _context.Mascotas.Where(m => m.Activo); if(currentRol == "Usuario") { mascotasQuery = mascotasQuery.Where(m => m.Dueno.UsuarioId == currentUserId); } ViewData["MascotaId"] = new SelectList(mascotasQuery.Take(50), "Id", "Nombre", cita.MascotaId);
            var veterinarios = _context.Veterinarios.Where(v => v.Activo).Take(50).Select(v => new { v.Id, NombreCompleto = v.Nombre + " " + v.Apellido }).ToList();
            ViewData["VeterinarioId"] = new SelectList(veterinarios, "Id", "NombreCompleto", cita.VeterinarioId);
            ViewData["SucursalId"] = new SelectList(_context.Sucursales.Where(s => s.Activo).Take(50), "Id", "Nombre", cita.SucursalId);
            return View(cita);
        }

        // GET: Citas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cita = await _context.Citas.FindAsync(id);
            if (cita == null || !cita.Activo) return NotFound();
            
            var currentUserId = HttpContext.Session.GetInt32("UserId"); var currentRol = HttpContext.Session.GetString("Rol"); var mascotasQuery = _context.Mascotas.Where(m => m.Activo); if(currentRol == "Usuario") { mascotasQuery = mascotasQuery.Where(m => m.Dueno.UsuarioId == currentUserId); } ViewData["MascotaId"] = new SelectList(mascotasQuery.Take(50), "Id", "Nombre", cita.MascotaId);
            var veterinarios = _context.Veterinarios.Where(v => v.Activo).Take(50).Select(v => new { v.Id, NombreCompleto = v.Nombre + " " + v.Apellido }).ToList();
            ViewData["VeterinarioId"] = new SelectList(veterinarios, "Id", "NombreCompleto", cita.VeterinarioId);
            ViewData["SucursalId"] = new SelectList(_context.Sucursales.Where(s => s.Activo).Take(50), "Id", "Nombre", cita.SucursalId);
            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaCita,Motivo,Estado,MascotaId,VeterinarioId,SucursalId")] Cita cita)
        {
            if (id != cita.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Citas.FindAsync(id);
                    if (existing == null || !existing.Activo) return NotFound();

                    existing.FechaCita = cita.FechaCita;
                    existing.Motivo = cita.Motivo;
                    existing.Estado = cita.Estado;
                    existing.MascotaId = cita.MascotaId;
                    existing.VeterinarioId = cita.VeterinarioId;
                    existing.SucursalId = cita.SucursalId;
                    existing.FechaActualizacion = DateTime.UtcNow;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CitaExists(cita.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            var currentUserId = HttpContext.Session.GetInt32("UserId"); var currentRol = HttpContext.Session.GetString("Rol"); var mascotasQuery = _context.Mascotas.Where(m => m.Activo); if(currentRol == "Usuario") { mascotasQuery = mascotasQuery.Where(m => m.Dueno.UsuarioId == currentUserId); } ViewData["MascotaId"] = new SelectList(mascotasQuery.Take(50), "Id", "Nombre", cita.MascotaId);
            var veterinarios = _context.Veterinarios.Where(v => v.Activo).Take(50).Select(v => new { v.Id, NombreCompleto = v.Nombre + " " + v.Apellido }).ToList();
            ViewData["VeterinarioId"] = new SelectList(veterinarios, "Id", "NombreCompleto", cita.VeterinarioId);
            ViewData["SucursalId"] = new SelectList(_context.Sucursales.Where(s => s.Activo).Take(50), "Id", "Nombre", cita.SucursalId);
            return View(cita);
        }

        // GET: Citas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cita = await _context.Citas
                .Include(c => c.Mascota)
                .Include(c => c.Veterinario)
                .Include(c => c.Sucursal)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (cita == null) return NotFound();

            return View(cita);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita != null)
            {
                cita.Activo = false;
                cita.FechaEliminacion = DateTime.UtcNow;
                _context.Update(cita);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CitaExists(int id)
        {
            return _context.Citas.Any(e => e.Id == id && e.Activo);
        }
    }
}
