using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VeterinariaApp.Models
{
    public class Sucursal : EntityBase
    {
        [Required(ErrorMessage = "El nombre de la sucursal es obligatorio")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(250)]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ciudad es obligatoria")]
        public int CiudadId { get; set; }

        public Ciudad? Ciudad { get; set; }

        // Relación con Veterinarios y Citas
        public ICollection<Veterinario>? Veterinarios { get; set; }
        public ICollection<Cita>? Citas { get; set; }
    }
}
