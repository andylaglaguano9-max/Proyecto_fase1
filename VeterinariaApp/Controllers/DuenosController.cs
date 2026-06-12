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
    public class DuenosController : Controller
    {
        private readonly AppDbContext _context;

        public DuenosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Duenos
        public async Task<IActionResult> Index(int pagina = 1, string buscar = "")
        {
            int tamañoPagina = 10;
            var query = _context.Duenos.AsQueryable();

            if (!string.IsNullOrEmpty(buscar))
                query = query.Where(d => d.Nombre.Contains(buscar) || d.Apellido.Contains(buscar) || d.Correo.Contains(buscar));

            int total = await query.CountAsync();
            var datos = await query
                .OrderByDescending(d => d.FechaCreacion)
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .ToListAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamañoPagina);
            ViewBag.TotalRegistros = total;
            ViewBag.Buscar = buscar;

            return View(datos);
        }

        // GET: Duenos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dueno = await _context.Duenos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dueno == null)
            {
                return NotFound();
            }

            return View(dueno);
        }

        // GET: Duenos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Duenos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Telefono,Direccion,Correo,Activo,FechaCreacion")] Dueno dueno)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dueno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dueno);
        }

        // GET: Duenos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dueno = await _context.Duenos.FindAsync(id);
            if (dueno == null)
            {
                return NotFound();
            }
            return View(dueno);
        }

        // POST: Duenos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Telefono,Direccion,Correo,Activo,FechaCreacion")] Dueno dueno)
        {
            if (id != dueno.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dueno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DuenoExists(dueno.Id))
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
            return View(dueno);
        }

        // GET: Duenos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dueno = await _context.Duenos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dueno == null)
            {
                return NotFound();
            }

            return View(dueno);
        }

        // POST: Duenos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dueno = await _context.Duenos.FindAsync(id);
            if (dueno != null)
            {
                // Eliminación lógica: solo marcar como inactivo
                dueno.Activo = false;
                _context.Update(dueno);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DuenoExists(int id)
        {
            return _context.Duenos.Any(e => e.Id == id);
        }
    }
}
