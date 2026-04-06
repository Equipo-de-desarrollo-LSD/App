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

        public CalificacionAppService(IRepository<Calificaciones.Calificacion, Guid> repository, ICurrentUser currentUser): base(repository)
        {
                _currentUser = currentUser;
            }

   //     public Task<CalificacionDto> CalificarDestinoAsync(CrearCalificacionDto input)
   //     {
   //          throw new NotImplementedException();
   //     }

        public override async Task<CalificacionDto> CreateAsync(CrearCalificacionDto input)
            {
                if (!_currentUser.IsAuthenticated)
                    throw new AbpAuthorizationException();

                var entity = ObjectMapper.Map<CrearCalificacionDto, WayFinder.Calificaciones.Calificacion>(input);
                entity.UserId = _currentUser.GetId();
                await Repository.InsertAsync(entity);
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
        [AllowAnonymous] // Permitimos que cualquiera vea el promedio, aunque no esté logueado
        public async Task<double> GetPromedioAsync(Guid destinoId)
        {
            var query = await Repository.GetQueryableAsync();
            var calificaciones = query.Where(c => c.DestinoId == destinoId);

            if (!calificaciones.Any())
            {
                return 0.0; // Si nadie calificó, el promedio es 0
            }

            // Calculamos el promedio matemático del puntaje
            return calificaciones.Average(c => c.Puntaje);
        }
        // --- REQ 5.5: LISTAR COMENTARIOS DE UN DESTINO ---
        [AllowAnonymous] // Permitimos que cualquiera lea los comentarios
        public async Task<List<CalificacionDto>> GetCalificacionesPorDestinoAsync(Guid destinoId)
        {
            var query = await Repository.GetQueryableAsync();

            var calificaciones = query
                .Where(c => c.DestinoId == destinoId)
                .ToList();

            return ObjectMapper.Map<List<WayFinder.Calificaciones.Calificacion>, List<CalificacionDto>>(calificaciones);
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

