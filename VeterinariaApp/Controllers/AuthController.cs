using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using VeterinariaApp.Data;
using VeterinariaApp.Models;

namespace VeterinariaApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Cifrado SHA256 de la contraseña ingresada por el usuario
            string hashedPassword = HashPassword(password);

            // Consulta LINQ para validación de credenciales. Se busca la coincidencia exacta de usuario,
            // permitiendo compatibilidad con datos base (en texto plano) o contraseñas previamente encriptadas.
            var user = _context.Usuarios
                .FirstOrDefault(u => u.Username == username && (u.Password == password || u.Password == hashedPassword) && u.Activo);

            if (user != null)
            {
                // Mecanismo de migración automática: si la contraseña estaba en texto plano, se encripta
                if (user.Password == password)
                {
                    user.Password = hashedPassword;
                    _context.SaveChanges();
                }

                // Generación de Sesión del Usuario: Almacenamiento seguro del contexto de autenticación
                // y los privilegios (Rol) en el servidor, implementando un control de acceso basado en roles (RBAC).
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Usuario", user.Username);
                HttpContext.Session.SetString("Rol", user.Rol);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos.";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string nombre, string apellido, string username, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                return View();
            }

            // Validar contraseña fuerte (mínimo 8, 1 número, 1 mayúscula, 1 minúscula)
            var passwordRegex = new System.Text.RegularExpressions.Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}$");
            if (!passwordRegex.IsMatch(password))
            {
                ViewBag.Error = "La contraseña debe tener al menos 8 caracteres, incluir un número, una mayúscula y una minúscula.";
                return View();
            }

            if (_context.Usuarios.Any(u => u.Username == username))
            {
                ViewBag.Error = "El usuario ya existe.";
                return View();
            }

            var newUser = new Usuario
            {
                Username = username,
                Password = HashPassword(password),
                Rol = "Usuario", // Rol explícito
                Activo = true,
                FechaCreacion = System.DateTime.UtcNow
            };

            _context.Usuarios.Add(newUser);
            _context.SaveChanges();

            // Crear el perfil de Dueño automáticamente
            var newDueno = new Dueno
            {
                Nombre = nombre,
                Apellido = apellido,
                Correo = username,
                Telefono = "0000000000", // Valor por defecto
                UsuarioId = newUser.Id,
                FechaCreacion = System.DateTime.UtcNow
            };
            
            _context.Duenos.Add(newDueno);
            _context.SaveChanges();

            // Auto login after registration
            HttpContext.Session.SetInt32("UserId", newUser.Id);
            HttpContext.Session.SetString("Usuario", newUser.Username);
            HttpContext.Session.SetString("Rol", newUser.Rol);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return System.BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
