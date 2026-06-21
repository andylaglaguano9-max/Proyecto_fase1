using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinariaApp.Models
{
    public class Veterinario : EntityBase
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        // Enlace al sistema de login
        public int? UsuarioId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [StringLength(100)]
        public string Especialidad { get; set; } = string.Empty;

        [StringLength(20)]
        public string Telefono { get; set; } = string.Empty;

        // Foránea a Sucursal
        [Required]
        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public Sucursal? Sucursal { get; set; }

        // Relación con Citas
        public ICollection<Cita>? Citas { get; set; }
    }
}
