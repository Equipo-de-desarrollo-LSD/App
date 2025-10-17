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

    }
}
