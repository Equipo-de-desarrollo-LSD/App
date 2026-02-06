using Autofac.Core;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Volo.Abp.Validation;
using WayFinder.DestinosTuristicosDTOs;
using WayFinder.EntityFrameworkCore;
using Xunit;
using static OpenIddict.Abstractions.OpenIddictConstants;
using NSubstitute.Extensions;
using WayFinder.DestinoTuristicos;

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

        // Mocks and Tests for BuscarCiudadAsync

        [Fact]
        public async Task SearchCiudadesAsync_ReturnsResults()
        {
            // Arrange
            var request = new BuscarCiudadRequestDto { NombreCiudad = "Test" };
            var expected = new BuscarCiudadResultDto
            {
                Ciudades = new List<CiudadDto> { new CiudadDto { Nombre = "TestCity", Pais = "TestCountry"} }
            };
            var repoMock = Substitute.For<IRepository<DestinoTuristico, Guid>>();
            var citySearchMock = Substitute.For<IBuscarCiudadService>();
            citySearchMock.SearchCitiesAsync(request).Returns(expected);
            var calificacionRepoMock = Substitute.For<IRepository<Calificaciones.Calificacion, Guid>>();
            var service = new DestinoTuristicoAppService(repoMock, citySearchMock, calificacionRepoMock);


            // Act
            var result = await service.BuscarCiudadAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Ciudades.Count.ShouldBe(1);
            result.Ciudades[0].Nombre.ShouldBe("TestCity");
        }

        [Fact]
        public async Task SearchCiudadesAsync_ReturnsEmpty()
        {
            // Arrange
            var request = new BuscarCiudadRequestDto { NombreCiudad = "NoMatch" };
            var expected = new BuscarCiudadResultDto { Ciudades = new List<CiudadDto>() };
            var repoMock = Substitute.For<IRepository<DestinoTuristico, Guid>>();
            var citySearchMock = Substitute.For<IBuscarCiudadService>();
            citySearchMock.SearchCitiesAsync(request).Returns(expected);
            var calificacionRepoMock = Substitute.For<IRepository<Calificaciones.Calificacion, Guid>>();
            var service = new DestinoTuristicoAppService(repoMock, citySearchMock, calificacionRepoMock);

            // Act
            var result = await service.BuscarCiudadAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Ciudades.ShouldBeEmpty();
        }

        [Fact]
        public async Task SearchCiudadesAsync_InvalidInput_ReturnsEmpty()
        {
            // Arrange
            var request = new BuscarCiudadRequestDto { NombreCiudad = "" };
            var expected = new BuscarCiudadResultDto { Ciudades = new List<CiudadDto>() };
            var repoMock = Substitute.For<IRepository<DestinoTuristico, Guid>>();
            var citySearchMock = Substitute.For<IBuscarCiudadService>();
            citySearchMock.SearchCitiesAsync(request).Returns(expected);
            var calificacionRepoMock = Substitute.For<IRepository<Calificaciones.Calificacion, Guid>>();
            var service = new DestinoTuristicoAppService(repoMock, citySearchMock, calificacionRepoMock);

            // Act
            var result = await service.BuscarCiudadAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Ciudades.ShouldBeEmpty();
        }

        [Fact]
        public async Task SearchCiudadesAsync_ApiError_ThrowsException()
        {
            // Arrange
            var request = new BuscarCiudadRequestDto { NombreCiudad = "Test" };
            var repoMock = Substitute.For<IRepository<DestinoTuristico, Guid>>();
            var citySearchMock = Substitute.For<IBuscarCiudadService>();
            citySearchMock
                .When(x => x.SearchCitiesAsync(request))
                .Do(x => { throw new Exception("API error"); });
            var calificacionRepoMock = Substitute.For<IRepository<Calificaciones.Calificacion, Guid>>();
            var service = new DestinoTuristicoAppService(repoMock, citySearchMock, calificacionRepoMock);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.BuscarCiudadAsync(request));
        }
        //


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

