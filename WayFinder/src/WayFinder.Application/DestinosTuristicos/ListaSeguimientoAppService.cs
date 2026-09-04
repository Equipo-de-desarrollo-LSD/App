using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using WayFinder.DestinosTuristicosDTOs;

namespace WayFinder.DestinosTuristicos
{
    //[AllowAnonymous] // Permitimos acceso sin token para tus pruebas actuales
    public class ListaSeguimientoAppService : WayFinderAppService, IListaSeguimientoAppService
    {
        private readonly IRepository<ListaSeguimiento, Guid> _listaSeguimientoRepository;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;

        public ListaSeguimientoAppService(IRepository<ListaSeguimiento, Guid> listaSeguimientoRepository,
            IRepository<DestinoTuristico, Guid> destinoRepository)
        {
            _listaSeguimientoRepository = listaSeguimientoRepository;
            _destinoRepository = destinoRepository;
        }

        public async Task SeguirDestinoAsync(Guid destinoId)
        {
            

            // ABP obtiene automáticamente el ID del token JWT del usuario logueado
            var userId = CurrentUser.Id;

            // Validación de seguridad: verificamos que haya un usuario autenticado
            if (userId == null)
            {
                throw new Volo.Abp.Authorization.AbpAuthorizationException("Debe estar iniciado sesión para seguir un destino.");
            }

            // Verificar si el Destino realmente existe
            var destinoExiste = await _destinoRepository.AnyAsync(x => x.Id == destinoId);
            if (!destinoExiste)
            {
                throw new EntityNotFoundException(typeof(DestinoTuristico), destinoId);
            }

            // Verificamos si ya lo sigue para no duplicar (usamos userId.Value porque es un Guid?)
            var existe = await _listaSeguimientoRepository.AnyAsync(x =>
                x.UsuarioId == userId.Value && x.DestinoId == destinoId);

            if (!existe)
            {
                await _listaSeguimientoRepository.InsertAsync(
                    new ListaSeguimiento(GuidGenerator.Create(), userId.Value, destinoId)
                );
            } 
        }

        public async Task DejarDeSeguirDestinoAsync(Guid destinoId)
        {
            // ABP obtiene automáticamente el ID del token JWT del usuario logueado
            var userId = CurrentUser.Id;

            // Validación de seguridad: verificamos que haya un usuario autenticado
            if (userId == null)
            {
                throw new Volo.Abp.Authorization.AbpAuthorizationException("Debe estar iniciado sesión para seguir un destino.");
            }

            await _listaSeguimientoRepository.DeleteAsync(x =>
                x.UsuarioId == userId.Value && x.DestinoId == destinoId);
        }
    }
}
