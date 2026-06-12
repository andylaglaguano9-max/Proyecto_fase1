using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinariaApp.Models
{
    public class Tratamiento
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        [Range(0, 99999.99)]
        public decimal Costo { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Foránea
        [Required]
        public int CitaId { get; set; }
        [ForeignKey("CitaId")]
        public Cita? Cita { get; set; }
    }
}
