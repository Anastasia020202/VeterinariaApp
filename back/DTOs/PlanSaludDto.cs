using System;
using System.ComponentModel.DataAnnotations;

namespace back.DTOs
{
    public class PlanSaludDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El tratamiento es obligatorio")]
        public int TratamientoId { get; set; }
        
        public string? NombreTratamiento { get; set; }
        
        public string Descripcion { get; set; } = string.Empty;
        
        [Range(0, double.MaxValue, ErrorMessage = "El costo debe ser un valor positivo")]
        public decimal Costo { get; set; }
        
        [Range(1, 120, ErrorMessage = "La duración debe estar entre 1 y 120 meses")]
        public int DuracionMeses { get; set; } = 12;
        
        [Range(0, 100, ErrorMessage = "El número de visitas debe estar entre 0 y 100")]
        public int VisitasIncluidas { get; set; } = 0;
        
        public bool IncluyeEmergencias { get; set; } = false;
        
        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
        public decimal PorcentajeDescuento { get; set; } = 0;
        
        public string DetallesCobertura { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateTime FechaInicio { get; set; }
        
        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        public DateTime FechaFin { get; set; }
        
        public bool Activo { get; set; }
    }
}
