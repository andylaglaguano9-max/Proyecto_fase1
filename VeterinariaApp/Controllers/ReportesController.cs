using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaApp.Data;
using VeterinariaApp.Filters;

namespace VeterinariaApp.Controllers
{
    [SessionAuthorize]
    public class ReportesController : Controller
    {
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string orden, string buscar = "", int pagina = 1)
        {
            // 1. Total general de ingresos (Sum)
            var totalIngresos = await _context.Tratamientos
                .Where(t => t.Activo)
                .SumAsync(t => t.Costo);

            // 2. Promedio de costo por tratamiento (Average)
            var promedioTratamiento = await _context.Tratamientos
                .Where(t => t.Activo)
                .AverageAsync(t => t.Costo);

            // 3. Citas por Estado (GroupBy, Count)
            var citasPorEstado = await _context.Citas
                .Where(c => c.Activo)
                .GroupBy(c => c.Estado)
                .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
                .ToDictionaryAsync(k => k.Estado, v => v.Cantidad);

            // 4. Top 10 Dueños con más mascotas (OrderByDescending, Take)
            var topDuenos = await _context.Duenos
                .Where(d => d.Activo)
                .Select(d => new
                {
                    Nombre = d.Nombre + " " + d.Apellido,
                    CantidadMascotas = d.Mascotas!.Count(m => m.Activo)
                })
                .OrderByDescending(d => d.CantidadMascotas)
                .Take(10)
                .ToListAsync();

            // 5. Total de registros activos e inactivos (Count)
            var totalActivos = await _context.Citas.CountAsync(c => c.Activo);
            var totalInactivos = await _context.Citas.CountAsync(c => !c.Activo);

            ViewBag.TotalIngresos = totalIngresos;
            ViewBag.PromedioTratamiento = promedioTratamiento;
            ViewBag.CitasPorEstado = citasPorEstado;
            ViewBag.TopDuenos = topDuenos;
            ViewBag.TotalActivos = totalActivos;
            ViewBag.TotalInactivos = totalInactivos;

            return View();
        }
    }
}
