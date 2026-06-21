using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinariaApp.Models
{
    public class DetalleFactura : EntityBase
    {
        [Required]
        public int FacturaId { get; set; }

        [ForeignKey("FacturaId")]
        public Factura? Factura { get; set; }

        [Required]
        [MaxLength(200)]
        public string Concepto { get; set; } = string.Empty;

        [Required]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }
    }
}
