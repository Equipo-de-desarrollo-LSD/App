using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using WayFinder.Calificaciones;
using WayFinder.DestinosTuristicosDTOs;
using Volo.Abp.Data;
using Microsoft.EntityFrameworkCore;

namespace WayFinder.Calificacion
{
    
        [Authorize]
        public class CalificacionAppService : CrudAppService<
        WayFinder.Calificaciones.Calificacion,
        DestinosTuristicosDTOs.CalificacionDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CrearCalificacionDto>, ICalificacionAppService


        {
            private readonly ICurrentUser _currentUser;
            private readonly IDataFilter _dataFilter;

        public CalificacionAppService(IRepository<Calificaciones.Calificacion, Guid> repository, ICurrentUser currentUser, IDataFilter dataFilter): base(repository)
        {
                _currentUser = currentUser;
                _dataFilter = dataFilter;
        }

   //     public Task<CalificacionDto> CalificarDestinoAsync(CrearCalificacionDto input)
   //     {
   //          throw new NotImplementedException();
   //     }

        public override async Task<CalificacionDto> CreateAsync(CrearCalificacionDto input)
            {
                if (!_currentUser.IsAuthenticated)
                    throw new AbpAuthorizationException("Debe estar logueado para calificar.");

                var entity = ObjectMapper.Map<CrearCalificacionDto, WayFinder.Calificaciones.Calificacion>(input);
                entity.UserId = _currentUser.GetId();
                await Repository.InsertAsync(entity, autoSave:true);
                return ObjectMapper.Map<WayFinder.Calificaciones.Calificacion, CalificacionDto>(entity);
            }
        // --- REQ 5.3: ELIMINAR CALIFICACIÓN PROPIA ---
        public override async Task DeleteAsync(Guid id)
        {
            var entity = await Repository.GetAsync(id);

            // Validamos que el usuario logueado sea el dueño de la calificación
            if (entity.UserId != _currentUser.GetId())
            {
                throw new AbpAuthorizationException("Solo puedes eliminar tus propias calificaciones.");
            }

            await base.DeleteAsync(id);
        }
        // --- REQ 5.4: CONSULTAR PROMEDIO ---
        [AllowAnonymous]
        public async Task<double> GetPromedioAsync(Guid destinoId)
        {
            using (_dataFilter.Disable<Volo.Abp.Auditing.ICreationAuditedObject>())
            {
                // CAMBIO AQUÍ: Obtenemos el queryable y filtramos manualmente
                var queryable = await Repository.GetQueryableAsync();
                var calificaciones = queryable.Where(c => c.DestinoId == destinoId).ToList();

                if (!calificaciones.Any())
                {
                    return 0.0;
                }

                return calificaciones.Average(c => c.Puntaje);
            }
        }

        // --- REQ 5.5: LISTAR COMENTARIOS DE UN DESTINO ---
        [AllowAnonymous]
        public async Task<List<CalificacionDto>> GetCalificacionesPorDestinoAsync(Guid destinoId)
        {
            using (_dataFilter.Disable<Volo.Abp.Auditing.ICreationAuditedObject>())
            {
                // 1. Obtenemos el queryable fresco
                var queryable = await Repository.GetQueryableAsync();

                // 2. Traemos todo a memoria (ToListAsync requiere "using Microsoft.EntityFrameworkCore;")
                var todasLasCalificaciones = await queryable.ToListAsync();

                // 3. Filtramos en memoria (donde el tipo Guid ya no es un problema para SQL)
                var calificaciones = todasLasCalificaciones.Where(c => c.DestinoId == destinoId).ToList();

                return ObjectMapper.Map<List<WayFinder.Calificaciones.Calificacion>, List<CalificacionDto>>(calificaciones);
            }
        }

        //  Task ICalificacionAppService.CalificarDestinoAsync(CrearCalificacionDto input)
        //  {
        //      return CalificarDestinoAsync(input);
        //  }



        /*
        public interface ICalificacionAppService : ICrudAppService<
         CalificacionDto,
         Guid,
         PagedAndSortedResultRequestDto,
         CrearCalificacionDto>
        {
            Task CalificarDestinoAsync(CrearCalificacionDto input);
        }
        */
    }

       
    }

