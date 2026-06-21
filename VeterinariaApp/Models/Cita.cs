using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinariaApp.Models
{
    public class Cita : EntityBase
    {
        public DateTime FechaCita { get; set; }

        [Required]
        [StringLength(250)]
        public string Motivo { get; set; } = string.Empty;

        [StringLength(50)]
        public string Estado { get; set; } = "Programada"; // Ej: Programada, Completada, Cancelada

        // Foráneas
        [Required]
        public int MascotaId { get; set; }
        [ForeignKey("MascotaId")]
        public Mascota? Mascota { get; set; }

        [Required]
        public int VeterinarioId { get; set; }
        [ForeignKey("VeterinarioId")]
        public Veterinario? Veterinario { get; set; }

        [Required]
        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public Sucursal? Sucursal { get; set; }

        // Relaciones
        public ICollection<Tratamiento>? Tratamientos { get; set; }
    }
}
