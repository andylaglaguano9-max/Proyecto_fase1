using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VeterinariaApp.Models
{
    public class Dueno : EntityBase
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        // Enlace al sistema de login
        public int? UsuarioId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(200)]
        public string Direccion { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string Correo { get; set; } = string.Empty;

        // Relación con mascotas
        public ICollection<Mascota>? Mascotas { get; set; }
    }
}
