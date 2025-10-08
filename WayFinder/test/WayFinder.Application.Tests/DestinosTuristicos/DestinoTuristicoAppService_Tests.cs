using System;
using System.Threading.Tasks;
using WayFinder.DestinosTuristicosDTOs;
using Shouldly;
using Volo.Abp.Modularity;
using Xunit;
using NSubstitute;

namespace WayFinder.DestinosTuristicos
{
    public abstract class DestinoTurisiticoAppService_Tests<TStartupModule> : WayFinder.WayFinderTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly IDestinoTuristicoAppService _services;

        protected DestinoTurisiticoAppService_Tests()
        {
            _services = GetRequiredService<IDestinoTuristicoAppService>();
        }

        [Fact]
        public async Task CrearAsyncShould_CreateDestinosTuristicosDto()
        {// arrange, que necesito para ejecutar el metodo
            var input = new GuardarDestinos
            { 
                nombre = "Playa Paraíso",
                foto = "playa_paraiso.jpg", // ← necesario
                pais = new PaisDto { nombre = "España", poblacion = 49000000 },
                coordenadas = new CoordenadasDto { latitud = 36.7213, longitud = -4.4214 },
                ultimaActualizacion = DateTime.Now

            };

            //act, cuando se ejectuta la accion que queremos probar
           // _services.ShouldNotBeNull();
            var result = await _services.CreateAsync(input);

            //assert, verificar que el resultado es el esperado
            /*result.ShouldNotBeNull();
            result.nombre.ShouldBe("Playa Paraíso");
            result.pais.nombre.ShouldBe("España");
            result.foto.ShouldBe(input.foto);
            result.coordenadas.latitud.ShouldBe(input.coordenadas.latitud);
            result.coordenadas.longitud.ShouldBe(input.coordenadas.longitud);
            result.Id.ShouldNotBe(Guid.Empty);*/
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
            result.nombre.ShouldBe(input.nombre);
            result.foto.ShouldBe(input.foto);
            result.pais.nombre.ShouldBe(input.pais.nombre);
            result.coordenadas.latitud.ShouldBe(input.coordenadas.latitud);
            result.coordenadas.longitud.ShouldBe(input.coordenadas.longitud);
            result.ultimaActualizacion.ShouldBe(input.ultimaActualizacion);
        }

    }
}
