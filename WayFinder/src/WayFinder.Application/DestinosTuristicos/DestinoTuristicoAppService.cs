using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Newtonsoft.Json.Linq;
using Volo.Abp.Domain.Repositories;
using WayFinder.Calificaciones;
using WayFinder.DestinosTuristicos;
using WayFinder.DestinosTuristicosDTOs;


namespace WayFinder.DestinoTuristicos;
[Authorize] // asegura que nadie que no esté logueado pueda llamar a ningún método de este servicio
public class DestinoTuristicoAppService :
    CrudAppService<
        DestinoTuristico, //The Book entity
        DestinoTuristicoDto, //Used to show books
        Guid, //Primary key of the book entity
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        GuardarDestinos>, //Used to create/update a book
        DestinosTuristicosDTOs.IDestinoTuristicoAppService//implement the IBookAppService
{
    private readonly IRepository<DestinoTuristico, Guid> _repository;
    private readonly IBuscarCiudadService _buscarCiudadService;
    private readonly IRepository<Calificaciones.Calificacion, Guid> _calificacionRepository;
    private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;
    private readonly IEventosService _eventosService;
    private readonly IRepository<WayFinder.Eventos.Evento, Guid> _eventoRepository;

    public DestinoTuristicoAppService(IRepository<DestinoTuristico, Guid> repository, IBuscarCiudadService buscarCiudadService, IRepository<Calificaciones.Calificacion, Guid> calificacionRepository,
             IRepository<DestinoTuristico, Guid> destinoRepository, IEventosService eventosService, IRepository<WayFinder.Eventos.Evento, Guid> eventoRepository)
        : base(repository)

    {
        _repository = repository;
        _buscarCiudadService = buscarCiudadService;
        _calificacionRepository = calificacionRepository;
        _destinoRepository = destinoRepository;
        _eventosService = eventosService;
        _eventoRepository = eventoRepository;
    }

    // Este es el constructor que usan tus tests. ¡Ahora sí guarda los datos!
    public DestinoTuristicoAppService(
        IRepository<DestinoTuristico, Guid> repository,
        IBuscarCiudadService citySearchMock,
        IRepository<Calificaciones.Calificacion, Guid> calificacionRepoMock)
        : base(repository)
    {
        _repository = repository;
        _buscarCiudadService = citySearchMock; // <--- AQUÍ ESTABA EL ERROR (antes no se asignaba)
        _calificacionRepository = calificacionRepoMock;
    }

    public async Task<BuscarCiudadResultDto> BuscarCiudadAsync(BuscarCiudadRequestDto request)
    {
        return await _buscarCiudadService.SearchCitiesAsync(request);
    }
    //alta
    // Asegúrate de tener estos using arriba:
    // using WayFinder.DestinosTuristicos; 
    // using WayFinder.Dominios; (Si Pais está ahí)

    public async Task<DestinoTuristicoDto> Crear(GuardarDestinos input)
    {
        // 1. Validaciones básicas
        if (string.IsNullOrWhiteSpace(input.Nombre))
        {
            throw new ArgumentException("El nombre no puede estar vacío.");
        }

        // 2. Preparar los "ingredientes" complejos (Value Objects)
        // Asumo que tu constructor de Pais y Coordenadas es simple
        var nuevoPais = new Pais(input.PaisNombre, input.PaisPoblacion);
        var nuevasCoordenadas = new Coordenadas(input.CoordenadasLatitud, input.CoordenadasLongitud);

        // 3. CREACIÓN MANUAL (Aquí está la magia para arreglar el error 500)
        // Usamos el constructor que TÚ creaste (el que pide ID)
        var nuevoDestino = new DestinoTuristico(GuidGenerator.Create())
        {
            // Asignamos las propiedades una a una
            nombre = input.Nombre, 
            foto = input.Foto,     
            Pais = nuevoPais,      
            Coordenadas = nuevasCoordenadas,
            UltimaActualizacion = DateTime.Now
        };

        // 4. Guardar en Base de Datos
        var destinoGuardado = await _repository.InsertAsync(nuevoDestino);

        // 5. Convertir a DTO para responder (Esto sí lo puede hacer AutoMapper de vuelta)
        return ObjectMapper.Map<DestinoTuristico, DestinoTuristicoDto>(destinoGuardado);
    }
    /* public async Task<DestinoTuristicoDto> Crear(GuardarDestinos input)

     {
         if (string.IsNullOrWhiteSpace(input.Nombre))
         {
             throw new ArgumentException("El nombre no puede estar vacío.");
         }
         var DestinoTuristico = await _repository.InsertAsync(ObjectMapper.Map<GuardarDestinos, DestinoTuristico>(input));
         return ObjectMapper.Map<DestinoTuristico, DestinoTuristicoDto>(DestinoTuristico);
     }*/

    //listar
    public async Task<List<DestinoTuristicoDto>> GetAllDestinosTuristicosAsync()
    {
        var destinos = await _repository.GetListAsync();
        return ObjectMapper.Map<List<DestinoTuristico>, List<DestinoTuristicoDto>>(destinos);
    }

    public async Task<BuscarCiudadResultDto> BuscarCiudades(BuscarCiudadRequestDto request)
    {
        // El AppService no sabe CÓMO se buscan.
        // Simplemente delega el trabajo al servicio que inyectó.
        // Esto cumple con el Punto 4: "utilice la interfaz para buscar ciudades".
        return await _buscarCiudadService.SearchCitiesAsync(request);
    }
    // Asegúrate de ponerle "Async" aquí también
    // Recuerda inyectar tu servicio GeoDbBuscarCiudadService en el constructor si no lo tienes
    // private readonly GeoDbBuscarCiudadService _geoDbService;

    public async Task<FiltrarCiudadesResultDto> FiltrarCiudadesAsync(FiltrarCiudadesRequestDto input)
    {
        // El AppService solo delega. ¡Así se hace!
        return await _buscarCiudadService.FiltrarCiudadesExternasAsync(input);
    }

    public async Task<DetalleCiudadDto> GetDetalleCiudadAsync(int id)
    {
        // Primero, buscamos los detalles básicos de la ciudad (igual que antes)
        var detalle = await _buscarCiudadService.ObtenerDetalleCiudadAsync(id);

        if (detalle != null && detalle.Coordenadas != null)
        {
            // Buscamos si la ciudad existe localmente como un Destino Turistico (haciendo match por nombre o coordenadas)
            // Asumiremos que el nombre es suficientemente único para este prototipo o validamos coordenadas
            var destinoLocal = await _destinoRepository.FirstOrDefaultAsync(d => 
                d.nombre == detalle.Nombre);

            if (destinoLocal != null)
            {
                // Si existe localmente, buscamos los eventos en la base de datos
                var eventosLocales = await _eventoRepository.GetListAsync(e => e.DestinoTuristicoId == destinoLocal.Id);
                
                detalle.Eventos = eventosLocales.Select(e => new WayFinder.DestinosTuristicosDTOs.Eventos.EventoDto
                {
                    IdExterno = e.IdExterno,
                    Nombre = e.Nombre,
                    UrlTicket = e.UrlTicket,
                    FechaInicio = e.FechaInicio,
                    ImagenUrl = e.ImagenUrl,
                    Lugar = e.Lugar
                }).ToList();
            }
            else
            {
                // Fallback: Si no existe localmente, buscamos en vivo en TicketMaster
                var eventosResult = await _eventosService.ObtenerEventosPorCoordenadasAsync(
                    detalle.Coordenadas.latitud,
                    detalle.Coordenadas.longitud
                );

                if (eventosResult != null && eventosResult.Eventos != null)
                {
                    detalle.Eventos = eventosResult.Eventos;
                }
            }
        }

        return detalle;
    }
}

