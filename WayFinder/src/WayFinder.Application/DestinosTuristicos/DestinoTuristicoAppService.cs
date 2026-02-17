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

    public DestinoTuristicoAppService(IRepository<DestinoTuristico, Guid> repository, IBuscarCiudadService buscarCiudadService, IRepository<Calificaciones.Calificacion, Guid> calificacionRepository,
             IRepository<DestinoTuristico, Guid> destinoRepository)
        : base(repository)

    {
        _repository = repository;
        _buscarCiudadService = buscarCiudadService;
        _calificacionRepository = calificacionRepository;
        _destinoRepository = destinoRepository;

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
    public async Task<DestinoTuristicoDto> Crear(GuardarDestinos input)
 
    {
        if (string.IsNullOrWhiteSpace(input.Nombre))
        {
            throw new ArgumentException("El nombre no puede estar vacío.");
        }
        var DestinoTuristico = await _repository.InsertAsync(ObjectMapper.Map<GuardarDestinos, DestinoTuristico>(input));
        return ObjectMapper.Map<DestinoTuristico, DestinoTuristicoDto>(DestinoTuristico);
    }

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


}
