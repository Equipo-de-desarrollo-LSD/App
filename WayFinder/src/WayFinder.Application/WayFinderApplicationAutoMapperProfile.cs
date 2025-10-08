using WayFinder.DestinosTuristicos;
using AutoMapper;
using WayFinder.DestinosTuristicosDTOs;

namespace WayFinder;

public class WayFinderApplicationAutoMapperProfile : Profile
{
    public WayFinderApplicationAutoMapperProfile()
    { 
        CreateMap<DestinoTuristico, DestinoTuristicoDto>();
        CreateMap<GuardarDestinos, DestinoTuristico>();
        CreateMap<PaisDto, Pais>();
        CreateMap<CoordenadasDto, Coordenadas>();
        // and vice versa
        CreateMap<Pais, PaisDto>();
        CreateMap<Coordenadas, CoordenadasDto>();
        CreateMap<DestinoTuristicoDto, DestinoTuristico>();
        CreateMap<DestinoTuristico, GuardarDestinos>();
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
    }
}

