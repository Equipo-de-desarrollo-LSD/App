using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WayFinder.DestinosTuristicosDTOs;

namespace WayFinder.Calificacion
{
    public interface ICalificacionAppService : ICrudAppService<
         CalificacionDto,
         Guid,
         PagedAndSortedResultRequestDto,
         CrearCalificacionDto>
    {
        Task CalificarDestinoAsync(CrearCalificacionDto input);
    }
}
