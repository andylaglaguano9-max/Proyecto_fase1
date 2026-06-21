using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VeterinariaApp.Services;

namespace VeterinariaApp.Controllers
{
    public class DataController : Controller
    {
        private readonly DataSeederService _seederService;

        public DataController(DataSeederService seederService)
        {
            _seederService = seederService;
        }

        [HttpGet]
        public IActionResult Seed()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RunSeed()
        {
            try
            {
                await _seederService.SeedDataAsync();
                TempData["Message"] = "Se han generado 500,000 registros correctamente.";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = "Error al generar datos: " + ex.Message;
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
