using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace back.Models
{
    public class Tratamiento
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public decimal CostoEstandar { get; set; } = 0;
        public string? RequisitosVeterinarios { get; set; }
        public string? CuidadosPosterior { get; set; }
        public string? MedicacionTipica { get; set; }
        public int DuracionEstimadaMinutos { get; set; } = 30;
        public string? IconoNombre { get; set; }
        public string? CodigoColor { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? UsuarioId { get; set; }
        public IdentityUser? Usuario { get; set; }
        public ICollection<ConsultaVeterinaria>? Consultas { get; set; }
        public ICollection<PlanSalud>? PlanesSalud { get; set; }
    }
}
