using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using WayFinder.DestinosTuristicos;

namespace WayFinder.Favoritos
{
    [Authorize] // ¡IMPORTANTE! Solo usuarios logueados pueden tener favoritos
    public class DestinoFavoritoAppService : ApplicationService, IDestinoFavoritoAppService
    {
        private readonly IRepository<DestinoFavorito, Guid> _favoritoRepository;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;

        public DestinoFavoritoAppService(
            IRepository<DestinoFavorito, Guid> favoritoRepository,
            IRepository<DestinoTuristico, Guid> destinoRepository)
        {
            _favoritoRepository = favoritoRepository;
            _destinoRepository = destinoRepository;
        }

        // 6.1 Agregar a favoritos
        public async Task CreateAsync(CreateDestinoFavoritoDto input)
        {
            // Verificamos si ya existe para no dar error de base de datos feo
            var existe = await _favoritoRepository.AnyAsync(x =>
                x.DestinoTuristicoId == input.DestinoTuristicoId &&
                x.CreatorId == CurrentUser.Id);

            if (existe)
            {
                return; // Ya es favorito, no hacemos nada (o podrías lanzar excepción)
            }

            var favorito = new DestinoFavorito(GuidGenerator.Create(), input.DestinoTuristicoId);
            await _favoritoRepository.InsertAsync(favorito);
        }

        // 6.2 Eliminar de favoritos (usando el ID del Destino, que es más cómodo para el Frontend)
        public async Task DeleteByDestinoIdAsync(Guid destinoId)
        {
            var favorito = await _favoritoRepository.FirstOrDefaultAsync(x =>
                x.DestinoTuristicoId == destinoId &&
                x.CreatorId == CurrentUser.Id);

            if (favorito != null)
            {
                await _favoritoRepository.DeleteAsync(favorito);
            }
        }

        // 6.3 Listar MIS favoritos (con datos del destino cruzados)
        public async Task<PagedResultDto<DestinoFavoritoDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            // 1. Obtener los favoritos del usuario actual
            var query = await _favoritoRepository.GetQueryableAsync();
            query = query.Where(x => x.CreatorId == CurrentUser.Id);

            var totalCount = await AsyncExecuter.CountAsync(query);

            // Paginación
            var favoritos = await AsyncExecuter.ToListAsync(
                query.OrderByDescending(x => x.CreationTime)
                     .PageBy(input.SkipCount, input.MaxResultCount)
            );

            if (!favoritos.Any())
            {
                return new PagedResultDto<DestinoFavoritoDto>(0, new List<DestinoFavoritoDto>());
            }

            // 2. Extraer los IDs de los destinos
            var destinoIds = favoritos.Select(x => x.DestinoTuristicoId).Distinct().ToList();

            // 3. Traer la información completa de esos destinos (Nombre, Foto, Pais)
            var destinos = await _destinoRepository.GetListAsync(x => destinoIds.Contains(x.Id));

            // 4. Cruzar los datos (Mapeo manual + AutoMapper)
            var dtos = favoritos.Select(fav =>
            {
                var dto = ObjectMapper.Map<DestinoFavorito, DestinoFavoritoDto>(fav);

                // Buscamos el destino correspondiente en la lista que trajimos
                var infoDestino = destinos.FirstOrDefault(d => d.Id == fav.DestinoTuristicoId);
                if (infoDestino != null)
                {
                    dto.NombreDestino = infoDestino.nombre;
                    dto.FotoDestino = infoDestino.foto;
                    dto.PaisDestino = infoDestino.Pais?.nombre ?? "Desconocido";
                }
                return dto;
            }).ToList();

            return new PagedResultDto<DestinoFavoritoDto>(totalCount, dtos);
        }

        // Extra: Helper para saber si un botón debe estar pintado o no
        public async Task<bool> IsFavoritoAsync(Guid destinoId)
        {
            return await _favoritoRepository.AnyAsync(x =>
                x.DestinoTuristicoId == destinoId &&
                x.CreatorId == CurrentUser.Id);
        }
    }
}