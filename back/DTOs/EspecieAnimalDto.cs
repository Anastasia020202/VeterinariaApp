using System;
using System.ComponentModel.DataAnnotations;

namespace back.DTOs
{
    public class EspecieAnimalDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; } = null!;
        
        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Descripcion { get; set; } = null!;
        
        public bool Activo { get; set; }
        
        public string ProblemasComunes { get; set; } = string.Empty;
        
        public string InstruccionesCuidado { get; set; } = string.Empty;
        
        public DateTime FechaCreacion { get; set; }
        
        // Propiedades de navegación calculadas para la UI
        public int? NumeroConsultas { get; set; }
    }
}
