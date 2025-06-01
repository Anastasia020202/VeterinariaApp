using AutoMapper;
using back.Models;
using back.DTOs;

namespace back {
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeos para EspecieAnimal
            CreateMap<EspecieAnimal, EspecieAnimalDto>().ReverseMap();

            // Mapeos para Tratamiento
            CreateMap<Tratamiento, TratamientoDto>().ReverseMap();

            // Mapeos para ConsultaVeterinaria
            CreateMap<ConsultaVeterinaria, ConsultaVeterinariaDto>().ReverseMap();

            // Mapeos para PlanSalud
            CreateMap<PlanSalud, PlanSaludDto>().ReverseMap();

            // Puedes agregar más mapeos aquí (Consultas, Otros modelos, etc.)
        }
    }
}
