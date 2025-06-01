using System;
using Microsoft.AspNetCore.Identity;

namespace back.Models
{
    public class ConsultaVeterinaria
    {
        public int Id { get; set; }
        public int EspecieAnimalId { get; set; }
        public int TratamientoId { get; set; }
        public string NombreMascota { get; set; } = null!;
        public string NombrePropietario { get; set; } = null!;
        public string InformacionContacto { get; set; } = null!;
        public decimal Costo { get; set; }
        public string Descripcion { get; set; } = null!;
        public string NotasTratamiento { get; set; } = string.Empty;
        public string Prescripcion { get; set; } = string.Empty;
        public DateTime FechaConsulta { get; set; }
        public string UsuarioId { get; set; } = null!;

        public EspecieAnimal? EspecieAnimal { get; set; }
        public Tratamiento? Tratamiento { get; set; }
        public IdentityUser? Usuario { get; set; }
    }
}
