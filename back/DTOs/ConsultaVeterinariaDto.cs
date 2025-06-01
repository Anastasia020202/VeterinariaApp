using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using back.Converters;

namespace back.DTOs
{
    public class ConsultaVeterinariaDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La especie animal es obligatoria")]
        public int EspecieAnimalId { get; set; }

        [Required(ErrorMessage = "El tratamiento es obligatorio")]
        public int TratamientoId { get; set; }

        [Required(ErrorMessage = "El nombre de la mascota es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string NombreMascota { get; set; } = null!;

        [Required(ErrorMessage = "El nombre del propietario es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string NombrePropietario { get; set; } = null!;

        [Required(ErrorMessage = "La información de contacto es obligatoria")]
        public string InformacionContacto { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "El costo debe ser un valor positivo")]
        public decimal Costo { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Descripcion { get; set; } = null!;

        public string NotasTratamiento { get; set; } = string.Empty;
        public string Prescripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de consulta es obligatoria")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime FechaConsulta { get; set; }

        // Propiedades para UI
        public string? NombreEspecieAnimal { get; set; }
        public string? NombreTratamiento { get; set; }
    }
}
