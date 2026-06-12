using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VeterinariaApp.Models
{
    public class Dueno
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [StringLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(200)]
        public string Direccion { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string Correo { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relación con mascotas
        public ICollection<Mascota>? Mascotas { get; set; }
    }
}
