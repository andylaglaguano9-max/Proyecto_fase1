using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinariaApp.Models
{
    public class Mascota
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Range(0.01, 200)]
        public decimal Peso { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Foráneas
        [Required]
        public int DuenoId { get; set; }
        [ForeignKey("DuenoId")]
        public Dueno? Dueno { get; set; }

        [Required]
        public int EspecieId { get; set; }
        [ForeignKey("EspecieId")]
        public Especie? Especie { get; set; }

        // Relaciones
        public ICollection<Cita>? Citas { get; set; }
    }
}
