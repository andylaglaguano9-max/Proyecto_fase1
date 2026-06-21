using System.ComponentModel.DataAnnotations;

namespace VeterinariaApp.Models
{
    public class Usuario : EntityBase
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Rol { get; set; } = "User"; // e.g., Admin, User
    }
}
