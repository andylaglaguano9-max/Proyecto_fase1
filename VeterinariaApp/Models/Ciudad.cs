using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VeterinariaApp.Models
{
    public class Ciudad : EntityBase
    {
        [Required(ErrorMessage = "El nombre de la ciudad es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        // Relación con Sucursales
        public ICollection<Sucursal>? Sucursales { get; set; }
    }
}
