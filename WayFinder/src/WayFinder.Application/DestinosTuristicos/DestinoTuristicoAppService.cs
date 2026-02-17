using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
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

    public DestinoTuristicoAppService(IRepository<DestinoTuristico, Guid> repository, IBuscarCiudadService buscarCiudadService, IRepository<Calificaciones.Calificacion, Guid> calificacionRepository)
        : base(repository)

    {
        _repository = repository;
        _buscarCiudadService = buscarCiudadService;
        _calificacionRepository = calificacionRepository;
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

    


   
}
