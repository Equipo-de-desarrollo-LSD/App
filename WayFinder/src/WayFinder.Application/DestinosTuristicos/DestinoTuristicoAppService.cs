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
using WayFinder.DestinosTuristicosDTOs;


namespace WayFinder.Calificaciones;
[Authorize] // asegura que nadie que no esté logueado pueda llamar a ningún método de este servicio
public class DestinoTuristicoAppService :
    CrudAppService<
        DestinoTuristico, //The Book entity
        DestinoTuristicoDto, //Used to show books
        Guid, //Primary key of the book entity
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        GuardarDestinos>, //Used to create/update a book
        DestinosTuristicosDTOs.IDestinoTuristicoAppService //implement the IBookAppService
{
    private readonly IRepository<DestinoTuristico, Guid> _repository;
    private readonly IBuscarCiudadService _buscarCiudadService;
    private readonly IRepository<Calificacion, Guid> _calificacionRepository;

    public DestinoTuristicoAppService(IRepository<DestinoTuristico, Guid> repository, IBuscarCiudadService buscarCiudadService, IRepository<Calificacion, Guid> calificacionRepository)
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

    public async Task  CalificarDestinoAsync(CalificacionDto input)
    {
        var userId = CurrentUser.Id.Value;

        // 
        var calificacionExistente = await _calificacionRepository.FirstOrDefaultAsync(
        c => c.DestinoId == input.DestinoId && c.UserId == userId
        );
        if (calificacionExistente != null)
        {
            throw new UserFriendlyException("¡Ya has calificado este destino!");
        }
        var calificacion = new Calificacion(
        GuidGenerator.Create(),
        input.DestinoId,
        userId, // <-- Aquí asociamos la calificación al usuario
        input.Puntaje,
        input.Comentario
        );

        // 3. Guardamos en la base de datos
        await _calificacionRepository.InsertAsync(calificacion);
    }

    public Task CalificarDestinoAsync(CrearCalificacionDto input)
    {
        throw new NotImplementedException();
    }

   
}
