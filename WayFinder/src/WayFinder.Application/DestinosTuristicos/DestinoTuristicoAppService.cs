using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WayFinder.DestinosTuristicosDTOs;


namespace WayFinder.DestinosTuristicos;
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
    

    public DestinoTuristicoAppService(IRepository<DestinoTuristico, Guid> repository, IBuscarCiudadService buscarCiudadService)
        : base(repository)

    {
        _repository = repository;
        _buscarCiudadService = buscarCiudadService;
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
}
