using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using WayFinder.NotificacionesDTOs;

namespace WayFinder.Notificaciones
{
    public class NotificacionAppService : WayFinderAppService, INotificacionAppService
    {
        private readonly IRepository<Notificacion, Guid> _notificacionRepository;

        public NotificacionAppService(IRepository<Notificacion, Guid> notificacionRepository)
        {
            _notificacionRepository = notificacionRepository;
        }

        // Obtener las notificaciones del usuario logueado
        public async Task<List<NotificacionDto>> GetListaAsync()
        {
            var notificaciones = await _notificacionRepository.GetListAsync(n => n.UsuarioId == CurrentUser.Id);
            return ObjectMapper.Map<List<Notificacion>, List<NotificacionDto>>(notificaciones);
        }

        // Marcar notificacion como leída
        public async Task MarcarComoLeidaAsync(Guid id)
        {
            // Verificar si el usuario esta logeado
            // ABP obtiene automáticamente el ID del token JWT del usuario logueado
            var userId = CurrentUser.Id;
            // Validación de seguridad: verificamos que haya un usuario autenticado
            if (userId == null)
            {
                throw new Volo.Abp.Authorization.AbpAuthorizationException("Debe estar iniciado sesión para seguir un destino.");
            }

            var notificacion = await _notificacionRepository.GetAsync(id);

            // Si no existe, lanzamos la excepción estándar de entidad no encontrada
            if (notificacion == null)
            {
                throw new EntityNotFoundException(typeof(Notificacion), id);
            }

            // Validación de seguridad: que la notificación pertenezca al usuario actual
            if (notificacion.UsuarioId != CurrentUser.Id)
            {
                throw new AbpAuthorizationException("No tiene permisos para modificar esta notificación.");
            }

            notificacion.Leido = true;
            await _notificacionRepository.UpdateAsync(notificacion);
        }

        // Marcar notificacion como no leída
        public async Task MarcarComoNoLeidaAsync(Guid id)
        {
            // Verificar si el usuario esta logeado
            // ABP obtiene automáticamente el ID del token JWT del usuario logueado
            var userId = CurrentUser.Id;
            // Validación de seguridad: verificamos que haya un usuario autenticado
            if (userId == null)
            {
                throw new Volo.Abp.Authorization.AbpAuthorizationException("Debe estar iniciado sesión para seguir un destino.");
            }

            var notificacion = await _notificacionRepository.GetAsync(id);
            // Si no existe, lanzamos la excepción estándar de entidad no encontrada
            if (notificacion == null)
            {
                throw new EntityNotFoundException(typeof(Notificacion), id);
            }

            // Validación de seguridad: que la notificación pertenezca al usuario actual
            if (notificacion.UsuarioId != CurrentUser.Id)
            {
                throw new AbpAuthorizationException("No tiene permisos para modificar esta notificación.");
            }

            notificacion.Leido = false;
            await _notificacionRepository.UpdateAsync(notificacion);
        }

        public async Task<int> GetCountNoLeidasAsync()
        {
            return await _notificacionRepository.CountAsync(n => n.UsuarioId == CurrentUser.Id && !n.Leido);
        }
    }
}
