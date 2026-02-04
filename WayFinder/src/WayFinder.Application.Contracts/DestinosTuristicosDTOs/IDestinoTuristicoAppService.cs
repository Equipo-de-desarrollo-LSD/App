using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WayFinder.DestinosTuristicosDTOs
{
    public interface IDestinoTuristicoAppService :
        ICrudAppService< //Defines CRUD methods
        DestinoTuristicoDto, //Used to show books
        Guid, //Primary key of the book entity
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        GuardarDestinos> //Used to create/update a book
    {
       Task<List<DestinoTuristicoDto>> GetAllDestinosTuristicosAsync();
       Task<DestinoTuristicoDto> Crear(GuardarDestinos input);

        // Metodo necesario para buscar ciudades
        Task<BuscarCiudadResultDto> BuscarCiudades(BuscarCiudadRequestDto request);
        //Task CalificarDestinoAsync(CalificacionDto input); // Nuevo método para calificar un destino turístico
        //Task CalificarDestinoAsync(CrearCalificacionDto input);
    }
}
