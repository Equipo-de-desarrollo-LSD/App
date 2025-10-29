using Autofac.Core;
using NSubstitute;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Volo.Abp.Validation;
using WayFinder.DestinosTuristicosDTOs;
using WayFinder.EntityFrameworkCore;
using Xunit;

namespace WayFinder.DestinosTuristicos
{
    public abstract class DestinoTurisiticoAppService_Tests<TStartupModule> : WayFinder.WayFinderTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly IDestinoTuristicoAppService _services;
        private readonly IDbContextProvider<WayFinderDbContext> _dbContextProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager ;
        protected DestinoTurisiticoAppService_Tests()
        {
            _services = GetRequiredService<IDestinoTuristicoAppService>();
            _dbContextProvider = GetRequiredService<IDbContextProvider<WayFinderDbContext>>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
        }

        [Fact]
        public async Task CrearAsyncShould_CreateDestinosTuristicosDto()
        {// arrange, que necesito para ejecutar el metodo
            var input = new GuardarDestinos
            { 
                Nombre = "Playa Paraíso",
                Foto = "playa_paraiso.jpg", // ← necesario
                PaisNombre = "España", 
                PaisPoblacion = 49000000 ,
                CoordenadasLatitud = 36.7213, 
                CoordenadasLongitud = -4.4214 ,
                UltimaActualizacion = DateTime.Now

            };
            //act, cuando se ejectuta la accion que queremos probar
           // _services.ShouldNotBeNull();
            var result = await _services.CreateAsync(input);

            //assert, verificar que el resultado es el esperado
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
            result.nombre.ShouldBe(input.Nombre);
            result.foto.ShouldBe(input.Foto);
            result.pais.nombre.ShouldBe(input.PaisNombre);
            result.coordenadas.latitud.ShouldBe(input.CoordenadasLatitud);
            result.coordenadas.longitud.ShouldBe(input.CoordenadasLongitud);
            result.ultimaActualizacion.ShouldBe(input.UltimaActualizacion);
        }
        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedDestinoTuristicoDto()
        {
            using var uow = _unitOfWorkManager.Begin();
            {
                // Arrange
                var input = new GuardarDestinos
                {
                    Nombre = "Montaña Mágica",
                    Foto = "montana_magica.jpg",
                    PaisNombre = "Chile",
                    PaisPoblacion = 19000000,
                    CoordenadasLatitud = -33.4489,
                    CoordenadasLongitud = -70.6693,
                    UltimaActualizacion = DateTime.Now
                };
                // Act
                var result = await _services.CreateAsync(input);
                var dbContext = await _dbContextProvider.GetDbContextAsync();
                var savedDestination = await dbContext.DestinosTuristicos.FindAsync(result.Id);

              
                // Assert
                savedDestination.ShouldNotBeNull();
                savedDestination.Id.ShouldNotBe(Guid.Empty);
                savedDestination.nombre.ShouldBe(input.Nombre);
                savedDestination.foto.ShouldBe(input.Foto);
                result.pais.nombre.ShouldBe(input.PaisNombre);
                result.coordenadas.latitud.ShouldBe(input.CoordenadasLatitud);
                result.coordenadas.longitud.ShouldBe(input.CoordenadasLongitud);
                result.ultimaActualizacion.ShouldBe(input.UltimaActualizacion);

            }
        }
        [Fact]
        public async Task CreateAsync_ShouldThrowExceptionWhenCountryIsNull()
        {
            // Arrange
            var input = new GuardarDestinos
            {
                Nombre = "Ciudad Fantasma",
                Foto = "ciudad_fantasma.jpg",
                PaisNombre = null, // País nulo para probar la excepción
                PaisPoblacion = 0,
                CoordenadasLatitud = 0,
                CoordenadasLongitud = 0,
                UltimaActualizacion = DateTime.Now
            };
            // Act & Assert
            await Should.ThrowAsync<AbpValidationException>(async () =>
            {
                await _services.CreateAsync(input);
            });
        }
      /*  [Fact]
        public async Task CreateAsync_ShouldPersistDestinationInDatabase()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                // Arrange
                var input = new GuardarDestinos
                {
                    Nombre = "Tokyo",
                    PaisNombre = "Japan"
                };

                // Act
                var result = await _services.CreateAsync(input);
                await uow.CompleteAsync();

                // Assert
                var dbContext = await _dbContextProvider.GetDbContextAsync();
                var savedDestination = await dbContext.DestinosTuristicos.FindAsync(result.Id);

                savedDestination.ShouldNotBeNull();
                savedDestination.nombre.ShouldBe(input.Nombre);
                savedDestination.Pais.nombre.ShouldBe(input.PaisNombre);

            }
        }
      */
    }
}
