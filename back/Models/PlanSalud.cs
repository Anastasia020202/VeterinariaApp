using System;
using Microsoft.AspNetCore.Identity;

namespace back.Models
{
    public class PlanSalud
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int TratamientoId { get; set; }
        public Tratamiento? Tratamiento { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Costo { get; set; }
        public int DuracionMeses { get; set; } = 12;
        public int VisitasIncluidas { get; set; } = 0;
        public bool IncluyeEmergencias { get; set; } = false;
        public decimal PorcentajeDescuento { get; set; } = 0;
        public string DetallesCobertura { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }
        public string? UsuarioId { get; set; }
        public IdentityUser? Usuario { get; set; }
    }
}
