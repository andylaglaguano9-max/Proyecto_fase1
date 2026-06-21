using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinariaApp.Models
{
    public class Vacuna : EntityBase
    {
        [Required(ErrorMessage = "El nombre de la vacuna es obligatorio")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Lote { get; set; } = string.Empty;

        [Required]
        public DateTime FechaAplicacion { get; set; }

        public DateTime? ProximaDosis { get; set; }

        [Required]
        public int MascotaId { get; set; }

        [ForeignKey("MascotaId")]
        public Mascota? Mascota { get; set; }

        [Required]
        public int VeterinarioId { get; set; }

        [ForeignKey("VeterinarioId")]
        public Veterinario? Veterinario { get; set; }
    }
}
