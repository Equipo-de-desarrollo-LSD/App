using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using WayFinder.Calificaciones;
using WayFinder.DestinosTuristicosDTOs;
using WayFinder.Permissions;

namespace WayFinder.Calificacion
{
    
        [Authorize]
        public class CalificacionAppService : CrudAppService<
        WayFinder.Calificaciones.Calificacion,
        DestinosTuristicosDTOs.CalificacionDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CrearCalificacionDto,
        ActualizarCalificacionDto>, ICalificacionAppService


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
        // --- REQ 5.3: ELIMINAR CALIFICACIÓN PROPIA (Y ADMIN) ---
        public override async Task DeleteAsync(Guid id)
        {
            // Primero intentamos buscar la entidad normalmente. Esto funcionará perfectamente para el autor.
            var entity = await Repository.FindAsync(id);

            // Si es nula, podría ser porque somos Administrador y el filtro de EF Core la está ocultando.
            if (entity == null)
            {
                var query = await Repository.GetQueryableAsync();
                entity = await query.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == id);
            }

            if (entity == null)
            {
                throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(WayFinder.Calificaciones.Calificacion), id);
            }

            var isAuthor = entity.UserId == _currentUser.GetId();
            var isAdmin = await AuthorizationService.IsGrantedAsync(WayFinderPermissions.DeleteCalificacion);
            
            // Si no es autor y no tiene permiso de admin, denegar
            if (!isAuthor && !isAdmin)
            {
                throw new AbpAuthorizationException("No tienes permiso para eliminar esta calificación.");
            }
            
            // Realizamos la eliminación física
            await Repository.DeleteAsync(entity, autoSave: true);
        }
        // --- REQ 5.4: CONSULTAR PROMEDIO ---
        [AllowAnonymous]
        public async Task<double> GetPromedioAsync(Guid destinoId)
        {
            using (_dataFilter.Disable<Volo.Abp.Auditing.ICreationAuditedObject>())
            {
                // CAMBIO AQUÍ: Obtenemos el queryable e ignoramos el filtro global de EF Core (IUserOwned) para poder ver las calificaciones de TODOS
                var queryable = await Repository.GetQueryableAsync();
                var calificaciones = await queryable.IgnoreQueryFilters().Where(c => c.DestinoId == destinoId).ToListAsync();

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

                // 2. Traemos todo a memoria e IGNORAMOS el filtro global IUserOwned para traer todas las calificaciones
                var todasLasCalificaciones = await queryable.IgnoreQueryFilters().ToListAsync();

                // 3. Filtramos en memoria (donde el tipo Guid ya no es un problema para SQL)
                var calificaciones = todasLasCalificaciones.Where(c => c.DestinoId == destinoId).ToList();

                return ObjectMapper.Map<List<WayFinder.Calificaciones.Calificacion>, List<CalificacionDto>>(calificaciones);
            }
        }

        // --- EDITAR CALIFICACION (Solo el autor) ---
        public override async Task<CalificacionDto> UpdateAsync(Guid id, ActualizarCalificacionDto input)
        {
            // 1. Validar autenticación
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException("Debe estar logueado para editar su calificación.");

            // 2. Obtener la entidad (el filtro IUserOwned asegura que solo el autor la encuentre, pero lo validamos igual por las dudas)
            var entity = await Repository.GetAsync(id); 

            if (entity.UserId != _currentUser.GetId())
            {
                throw new AbpAuthorizationException("No tienes permiso para editar esta calificación.");
            }

            // 3. Mapear los campos actualizables (Puntaje y Comentario). No mapeamos el DestinoId ni el UserId para que no lo cambien.
            entity.Puntaje = input.Puntaje;
            entity.Comentario = input.Comentario;
            
            await Repository.UpdateAsync(entity);
            return ObjectMapper.Map<WayFinder.Calificaciones.Calificacion, CalificacionDto>(entity);
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

