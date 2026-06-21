using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinariaApp.Models
{
    public class Venta : EntityBase
    {
        [Required]
        public DateTime FechaVenta { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Required]
        public int DuenoId { get; set; }

        [ForeignKey("DuenoId")]
        public Dueno? Dueno { get; set; }

        public ICollection<DetalleVenta>? Detalles { get; set; }
    }
}
