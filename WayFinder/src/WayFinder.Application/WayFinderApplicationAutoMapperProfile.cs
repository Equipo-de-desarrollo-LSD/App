using WayFinder.DestinosTuristicos;
using AutoMapper;
using WayFinder.DestinosTuristicosDTOs;

namespace WayFinder;

public class WayFinderApplicationAutoMapperProfile : Profile
{
    public WayFinderApplicationAutoMapperProfile()
    { 
        CreateMap<DestinoTuristico, DestinoTuristicoDto>();
        CreateMap<GuardarDestinos, DestinoTuristico>()
            .ForMember(dest => dest.Pais, // Para la propiedad 'Pais' de la entidad...
                       opt => opt.MapFrom(src => new Pais(src.PaisNombre, src.PaisPoblacion))) // ...crea un nuevo objeto Pais.
            .ForMember(dest => dest.Coordenadas, // Para la propiedad 'Coordenadas' de la entidad...
                       opt => opt.MapFrom(src => new Coordenadas(src.CoordenadasLatitud, src.CoordenadasLongitud))); // ...crea un nuevo objeto Coordenadas.

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
/* public WayFinderApplicationAutoMapperProfile()
{
    // --- MAPEO CORRECTO PARA LA CREACIÓN ---
    // Le dice a AutoMapper cómo construir los ValueObjects desde el DTO de entrada.
    CreateMap<GuardarDestinos, DestinoTuristico>()
        .ForMember(dest => dest.Pais, // Para la propiedad 'Pais' de la entidad...
                   opt => opt.MapFrom(src => new Pais(src.PaisNombre, src.PaisPoblacion))) // ...crea un nuevo objeto Pais.
        .ForMember(dest => dest.Coordenadas, // Para la propiedad 'Coordenadas' de la entidad...
                   opt => opt.MapFrom(src => new Coordenadas(src.CoordenadasLatitud, src.CoordenadasLongitud))); // ...crea un nuevo objeto Coordenadas.

    // --- MAPEO PARA LA RESPUESTA ---
    // Le dice a AutoMapper cómo convertir la entidad de vuelta a un DTO para la respuesta de la API.
    CreateMap<DestinoTuristico, DestinoTuristicoDto>();
}
*/
