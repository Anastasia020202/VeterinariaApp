using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace back.Models
{
    public class EspecieAnimal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public bool Activo { get; set; }
        public string ProblemasComunes { get; set; } = string.Empty;
        public string InstruccionesCuidado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public string? UsuarioId { get; set; }
        public IdentityUser? Usuario { get; set; }
        public ICollection<ConsultaVeterinaria>? Consultas { get; set; }
    }
}
