using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinariaApp.Models
{
    public class Factura : EntityBase
    {
        [Required]
        public DateTime FechaEmision { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Required]
        [MaxLength(50)]
        public string Estado { get; set; } = "Pendiente"; // Pagada, Pendiente, Anulada

        // Relación con Dueño
        [Required]
        public int DuenoId { get; set; }

        [ForeignKey("DuenoId")]
        public Dueno? Dueno { get; set; }

        // Relación 1 a N
        public ICollection<DetalleFactura>? Detalles { get; set; }
    }
}
