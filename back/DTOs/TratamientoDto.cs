using System;
using System.ComponentModel.DataAnnotations;

namespace back.DTOs
{
    public class TratamientoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Descripcion { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "El costo debe ser un valor positivo")]
        public decimal CostoEstandar { get; set; } = 0;

        public string? RequisitosVeterinarios { get; set; }

        public string? CuidadosPosterior { get; set; }

        public string? MedicacionTipica { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La duración debe ser mayor a cero")]
        public int DuracionEstimadaMinutos { get; set; } = 30;

        public string? IconoNombre { get; set; }

        public string? CodigoColor { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
