using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VeterinariaApp.Models
{
    public class Especie : EntityBase
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        // Relación con mascotas
        public ICollection<Mascota>? Mascotas { get; set; }
    }
}
