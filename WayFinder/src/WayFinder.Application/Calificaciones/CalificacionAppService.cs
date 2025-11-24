using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WayFinder.Calificaciones;
using WayFinder.DestinosTuristicosDTOs;
using Volo.Abp.Users;
using Volo.Abp;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace WayFinder.Calificacion
{
    
        [Authorize]
        public class CalificacionAppService : CrudAppService<
        WayFinder.Calificaciones.Calificacion,
        DestinosTuristicosDTOs.CalificacionDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CrearCalificacionDto>, CalificacionAppService.ICalificacionAppService


        {
            private readonly ICurrentUser _currentUser;

        public CalificacionAppService(IRepository<Calificaciones.Calificacion, Guid> repository, ICurrentUser currentUser): base(repository)
        {
                _currentUser = currentUser;
            }

        public Task<CalificacionDto> CalificarDestinoAsync(CrearCalificacionDto input)
        {
            throw new NotImplementedException();
        }

        public override async Task<CalificacionDto> CreateAsync(CrearCalificacionDto input)
            {
                if (!_currentUser.IsAuthenticated)
                    throw new AbpAuthorizationException();

                var entity = ObjectMapper.Map<CrearCalificacionDto, WayFinder.Calificaciones.Calificacion>(input);
                entity.UserId = _currentUser.GetId();
                await Repository.InsertAsync(entity);
                return ObjectMapper.Map<WayFinder.Calificaciones.Calificacion, CalificacionDto>(entity);
            }

        Task ICalificacionAppService.CalificarDestinoAsync(CrearCalificacionDto input)
        {
            return CalificarDestinoAsync(input);
        }

        public interface ICalificacionAppService : ICrudAppService<
         CalificacionDto,
         Guid,
         PagedAndSortedResultRequestDto,
         CrearCalificacionDto>
        {
            Task CalificarDestinoAsync(CrearCalificacionDto input);
        }
    }

        
    }

