using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VeterinariaApp.Models
{
    public class Especie
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relación con mascotas
        public ICollection<Mascota>? Mascotas { get; set; }
    }
}
