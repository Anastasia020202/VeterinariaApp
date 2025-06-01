using AutoMapper;
using back.DTOs;
using back.Models;

namespace back.MappingProfiles
{
    public class VeterinariaProfile : Profile
    {
        public VeterinariaProfile()
        {
            // EspecieAnimal mappings
            CreateMap<EspecieAnimal, EspecieAnimalDto>().ReverseMap();
            
            // Tratamiento mappings
            CreateMap<Tratamiento, TratamientoDto>().ReverseMap();
            
            // ConsultaVeterinaria mappings
            CreateMap<ConsultaVeterinaria, ConsultaVeterinariaDto>()
                .ForMember(dest => dest.NombreEspecieAnimal, opt => opt.Ignore())
                .ForMember(dest => dest.NombreTratamiento, opt => opt.Ignore());
                
            CreateMap<ConsultaVeterinariaDto, ConsultaVeterinaria>()
                .ForMember(dest => dest.EspecieAnimal, opt => opt.Ignore())
                .ForMember(dest => dest.Tratamiento, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario, opt => opt.Ignore());
            
            // PlanSalud mappings
            CreateMap<PlanSalud, PlanSaludDto>()
                .ForMember(dest => dest.NombreTratamiento, opt => opt.Ignore());
                
            CreateMap<PlanSaludDto, PlanSalud>()
                .ForMember(dest => dest.Tratamiento, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario, opt => opt.Ignore());
        }
    }
}
