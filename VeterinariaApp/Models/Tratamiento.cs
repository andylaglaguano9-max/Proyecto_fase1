using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinariaApp.Models
{
    public class Tratamiento : EntityBase
    {
        [Required]
        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        [Range(0, 99999.99)]
        public decimal Costo { get; set; }

        // Foránea
        [Required]
        public int CitaId { get; set; }
        [ForeignKey("CitaId")]
        public Cita? Cita { get; set; }
    }
}
