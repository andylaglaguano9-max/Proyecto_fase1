using System;
using System.ComponentModel.DataAnnotations;

namespace VeterinariaApp.Models
{
    // Clase abstracta que implementa el patrón de Diseño Data Transfer Object (DTO) Base.
    // Todas las entidades de dominio heredan de esta clase para asegurar una auditoría uniforme.
    public abstract class EntityBase
    {
        // Clave Primaria autonumérica universal para todas las tablas.
        [Key]
        public int Id { get; set; }

        // Implementación del patrón "Soft Delete" (Borrado Lógico).
        // Los registros nunca se eliminan físicamente de la base de datos (DELETE),
        // solo se actualiza este flag a 'false' para preservar la integridad referencial.
        public bool Activo { get; set; } = true;

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }

        public DateTime? FechaEliminacion { get; set; }
    }
}
