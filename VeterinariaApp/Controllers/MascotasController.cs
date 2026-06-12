using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VeterinariaApp.Data;
using VeterinariaApp.Models;

namespace VeterinariaApp.Controllers
{
    public class MascotasController : Controller
    {
        private readonly AppDbContext _context;

        public MascotasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Mascotas
        public async Task<IActionResult> Index(int pagina = 1, string buscar = "")
        {
            int tamañoPagina = 10;
            var query = _context.Mascotas.Include(m => m.Dueno).Include(m => m.Especie).AsQueryable();

            if (!string.IsNullOrEmpty(buscar))
                query = query.Where(m => m.Nombre.Contains(buscar) ||
                    m.Dueno!.Nombre.Contains(buscar) ||
                    m.Especie!.Nombre.Contains(buscar));

            int total = await query.CountAsync();
            var datos = await query
                .OrderByDescending(m => m.FechaCreacion)
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .ToListAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamañoPagina);
            ViewBag.TotalRegistros = total;
            ViewBag.Buscar = buscar;

            return View(datos);
        }

        // GET: Mascotas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mascota = await _context.Mascotas
                .Include(m => m.Dueno)
                .Include(m => m.Especie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mascota == null)
            {
                return NotFound();
            }

            return View(mascota);
        }

        // GET: Mascotas/Create
        public IActionResult Create()
        {
            var duenos = _context.Duenos.Select(d => new { d.Id, NombreCompleto = d.Nombre + " " + d.Apellido }).ToList();
            ViewData["DuenoId"] = new SelectList(duenos, "Id", "NombreCompleto");
            ViewData["EspecieId"] = new SelectList(_context.Especies, "Id", "Nombre");
            return View();
        }

        // POST: Mascotas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,FechaNacimiento,Peso,Activo,FechaCreacion,DuenoId,EspecieId")] Mascota mascota)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mascota);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var duenos2 = _context.Duenos.Select(d => new { d.Id, NombreCompleto = d.Nombre + " " + d.Apellido }).ToList();
            ViewData["DuenoId"] = new SelectList(duenos2, "Id", "NombreCompleto", mascota.DuenoId);
            ViewData["EspecieId"] = new SelectList(_context.Especies, "Id", "Nombre", mascota.EspecieId);
            return View(mascota);
        }

        // GET: Mascotas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota == null)
            {
                return NotFound();
            }
            var duenos3 = _context.Duenos.Select(d => new { d.Id, NombreCompleto = d.Nombre + " " + d.Apellido }).ToList();
            ViewData["DuenoId"] = new SelectList(duenos3, "Id", "NombreCompleto", mascota.DuenoId);
            ViewData["EspecieId"] = new SelectList(_context.Especies, "Id", "Nombre", mascota.EspecieId);
            return View(mascota);
        }

        // POST: Mascotas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,FechaNacimiento,Peso,Activo,FechaCreacion,DuenoId,EspecieId")] Mascota mascota)
        {
            if (id != mascota.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mascota);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MascotaExists(mascota.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var duenos4 = _context.Duenos.Select(d => new { d.Id, NombreCompleto = d.Nombre + " " + d.Apellido }).ToList();
            ViewData["DuenoId"] = new SelectList(duenos4, "Id", "NombreCompleto", mascota.DuenoId);
            ViewData["EspecieId"] = new SelectList(_context.Especies, "Id", "Nombre", mascota.EspecieId);
            return View(mascota);
        }

        // GET: Mascotas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mascota = await _context.Mascotas
                .Include(m => m.Dueno)
                .Include(m => m.Especie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mascota == null)
            {
                return NotFound();
            }

            return View(mascota);
        }

        // POST: Mascotas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota != null)
            {
                // Eliminación lógica: solo marcamos como inactivo
                mascota.Activo = false;
                _context.Update(mascota);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MascotaExists(int id)
        {
            return _context.Mascotas.Any(e => e.Id == id);
        }
    }
}
