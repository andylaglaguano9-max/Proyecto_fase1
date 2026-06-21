using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaApp.Data;
using VeterinariaApp.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace VeterinariaApp.Controllers
{
    public class ProductosController : Controller
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string orden, string buscar, int pagina = 1)
        {
            int registrosPorPagina = 50;
            var query = _context.Productos.Where(e => e.Activo).AsQueryable();

            
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
        public async Task<IActionResult> Create(Producto entity)
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

            var entity = await _context.Productos.FindAsync(id);
            if (entity == null || !entity.Activo) return NotFound();
            
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto entity)
        {
            if (id != entity.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var original = await _context.Productos.FindAsync(id);
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

            var entity = await _context.Productos.FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (entity == null) return NotFound();

            return View(entity);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var entity = await _context.Productos.FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (entity == null) return NotFound();

            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.Productos.FindAsync(id);
            if (entity != null)
            {
                entity.Activo = false;
                entity.FechaEliminacion = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comprar(int id)
        {
            var currentRol = HttpContext.Session.GetString("Rol");
            if (currentRol != "Usuario") return Unauthorized("Solo los clientes pueden realizar compras.");

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var dueno = await _context.Duenos.FirstOrDefaultAsync(d => d.UsuarioId == currentUserId && d.Activo);
            
            if (dueno == null)
            {
                TempData["Error"] = "No se encontró un perfil de cliente asociado a tu cuenta.";
                return RedirectToAction(nameof(Index));
            }

            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == id && p.Activo);
            if (producto == null) return NotFound();

            // Verificación y validación de inventario antes de procesar la transacción
            if (producto.Stock < 1)
            {
                TempData["Error"] = $"El producto '{producto.Nombre}' está agotado.";
                return RedirectToAction(nameof(Index));
            }

            // Inicio del bloque transaccional implícito de Entity Framework Core.
            // Paso 1: Actualización del inventario descontando las unidades adquiridas.
            producto.Stock -= 1;
            producto.FechaActualizacion = DateTime.UtcNow;
            _context.Update(producto);

            // Paso 2: Generación del registro maestro de la transacción comercial (Venta).
            var venta = new Venta
            {
                FechaVenta = DateTime.UtcNow,
                Total = producto.Precio,
                DuenoId = dueno.Id,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };
            _context.Ventas.Add(venta);
            
            // Persistencia inicial para obtener el ID autonumérico generado por PostgreSQL.
            await _context.SaveChangesAsync();

            // Crear Detalle
            var detalle = new DetalleVenta
            {
                VentaId = venta.Id,
                ProductoId = producto.Id,
                Cantidad = 1,
                PrecioUnitario = producto.Precio,
                Subtotal = producto.Precio,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };
            _context.DetallesVenta.Add(detalle);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"¡Compra de '{producto.Nombre}' realizada con éxito!";
            return RedirectToAction("Index", "Ventas");
        }
    }
}
