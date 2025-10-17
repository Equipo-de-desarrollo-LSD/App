using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayFinder.DestinosTuristicosDTOs;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Account;


namespace WayFinder.DestinosTuristicos;

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

    public DestinoTuristicoAppService(IRepository<DestinoTuristico, Guid> repository)
        : base(repository)

    {
        _repository = repository;
    }

    //alta
    public async Task<DestinoTuristicoDto> Crear(GuardarDestinos input)
 
    {
        if (string.IsNullOrWhiteSpace(input.Nombre))
        {
            throw new ArgumentException("El nombre no puede estar vacío.");
        }
        var DestinoTuristico = await _repository.InsertAsync(ObjectMapper.Map<GuardarDestinos, DestinoTuristico>(input));
        return ObjectMapper.Map<DestinoTuristico, DestinoTuristicoDto>(DestinoTuristico)
        ;


    }

    //listar
    public async Task<List<DestinoTuristicoDto>> GetAllDestinosTuristicosAsync()
    {
        var destinos = await _repository.GetListAsync();
        return ObjectMapper.Map<List<DestinoTuristico>, List<DestinoTuristicoDto>>(destinos);
    }
}
