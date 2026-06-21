using Microsoft.AspNetCore.Mvc;
using System.Linq;
using VeterinariaApp.Data;
using VeterinariaApp.Filters;
using VeterinariaApp.Models;

namespace VeterinariaApp.Controllers
{
    [SessionAuthorize] // Add Authorization
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var currentRol = HttpContext.Session.GetString("Rol");
            if (currentRol != "Admin")
            {
                return Unauthorized("Solo los administradores pueden gestionar usuarios.");
            }

            var usuarios = _context.Usuarios.OrderByDescending(u => u.FechaCreacion).ToList();
            return View(usuarios);
        }

        [HttpPost]
        public IActionResult CambiarRol(int id, string nuevoRol)
        {
            var currentRol = HttpContext.Session.GetString("Rol");
            if (currentRol != "Admin")
            {
                return Unauthorized();
            }

            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Validar que no se quite el rol Admin a sí mismo por error
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == usuario.Id && nuevoRol != "Admin")
            {
                TempData["Error"] = "No puedes quitarte el rol de Administrador a ti mismo.";
                return RedirectToAction(nameof(Index));
            }

            usuario.Rol = nuevoRol;
            usuario.FechaActualizacion = System.DateTime.UtcNow;

            // Si el rol es Doctor y no tiene Veterinario asociado, lo creamos
            if (nuevoRol == "Doctor" && !_context.Veterinarios.Any(v => v.UsuarioId == usuario.Id))
            {
                var sucursalId = _context.Sucursales.FirstOrDefault()?.Id ?? 1;
                
                var veterinario = new Veterinario
                {
                    Nombre = "Doctor",
                    Apellido = "Temporal",
                    Especialidad = "General",
                    Telefono = "000",
                    UsuarioId = usuario.Id,
                    SucursalId = sucursalId,
                    FechaCreacion = System.DateTime.UtcNow
                };
                _context.Veterinarios.Add(veterinario);
            }

            _context.SaveChanges();
            TempData["Success"] = $"Rol actualizado a {nuevoRol} exitosamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var currentRol = HttpContext.Session.GetString("Rol");
            if (currentRol != "Admin") return Unauthorized();

            var usuario = _context.Usuarios.Find(id);
            if (usuario == null) return NotFound();

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == usuario.Id)
            {
                TempData["Error"] = "No puedes desactivar tu propia cuenta.";
                return RedirectToAction(nameof(Index));
            }

            usuario.Activo = !usuario.Activo;
            usuario.FechaActualizacion = System.DateTime.UtcNow;
            _context.SaveChanges();

            TempData["Success"] = $"Estado de usuario {(usuario.Activo ? "activado" : "desactivado")} exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            var currentRol = HttpContext.Session.GetString("Rol");
            if (currentRol != "Admin") return Unauthorized();

            if (id == null) return NotFound();

            var usuario = _context.Usuarios.Find(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Username,Rol,Activo")] Usuario usuario)
        {
            var currentRol = HttpContext.Session.GetString("Rol");
            if (currentRol != "Admin") return Unauthorized();

            if (id != usuario.Id) return NotFound();

            var existing = _context.Usuarios.Find(id);
            if (existing == null) return NotFound();

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == usuario.Id && usuario.Rol != "Admin")
            {
                ModelState.AddModelError("Rol", "No puedes quitarte el rol de Administrador a ti mismo.");
            }

            if (ModelState.IsValid)
            {
                existing.Username = usuario.Username;
                existing.Rol = usuario.Rol;
                existing.Activo = usuario.Activo;
                existing.FechaActualizacion = System.DateTime.UtcNow;
                _context.SaveChanges();
                TempData["Success"] = "Usuario actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        public IActionResult Delete(int? id)
        {
            var currentRol = HttpContext.Session.GetString("Rol");
            if (currentRol != "Admin") return Unauthorized();

            if (id == null) return NotFound();

            var usuario = _context.Usuarios.Find(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var currentRol = HttpContext.Session.GetString("Rol");
            if (currentRol != "Admin") return Unauthorized();

            var usuario = _context.Usuarios.Find(id);
            if (usuario == null) return NotFound();

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == usuario.Id)
            {
                TempData["Error"] = "No puedes eliminar tu propia cuenta.";
                return RedirectToAction(nameof(Index));
            }

            // Realizamos borrado lógico
            usuario.Activo = false;
            usuario.FechaEliminacion = System.DateTime.UtcNow;
            _context.SaveChanges();

            TempData["Success"] = "Usuario eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
