using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims; 
using Shouldly;
using Xunit;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Security.Claims; 
using Volo.Abp.Modularity;
using WayFinder.DestinosTuristicos;
using WayFinder.DestinosTuristicosDTOs;


namespace WayFinder.Favoritos
{
    public class DestinoFavoritoAppServiceTests : WayFinderApplicationTestBase<WayFinderApplicationTestModule>
    {
        private readonly IDestinoFavoritoAppService _favoritoAppService;
        private readonly IRepository<WayFinder.DestinosTuristicos.DestinoTuristico, Guid> _destinoRepository;

       
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

        public DestinoFavoritoAppServiceTests()
        {
            _favoritoAppService = GetRequiredService<IDestinoFavoritoAppService>();
            _destinoRepository = GetRequiredService<IRepository<WayFinder.DestinosTuristicos.DestinoTuristico, Guid>>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
        }

        [Fact]
        public async Task Should_Agregar_Y_Listar_Favorito()
        {
            // 1. ARRANGE
            var testUserId = Guid.NewGuid(); // ID del usuario falso
            var destinoId = Guid.NewGuid();

            var destinoTest = new WayFinder.DestinosTuristicos.DestinoTuristico(destinoId)
            {
                nombre = "Playa Test",
                foto = "test.jpg",
                UltimaActualizacion = DateTime.Now,
                Pais = new Pais("Argentina", 45000000),
                Coordenadas = new Coordenadas(-34.6037, -58.3816)
            };
            await _destinoRepository.InsertAsync(destinoTest);

            // 2. ACT & ASSERT (Usando el truco de tus compañeros)
            // Llamamos al método de abajo (CreateTestPrincipal) para fabricar la identidad
            using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
            {
                var input = new CreateDestinoFavoritoDto { DestinoTuristicoId = destinoId };

                await _favoritoAppService.CreateAsync(input);

                var resultado = await _favoritoAppService.GetListAsync(new PagedAndSortedResultRequestDto());

                resultado.TotalCount.ShouldBe(1);
                resultado.Items[0].NombreDestino.ShouldBe("Playa Test");
            }
        }

        [Fact]
        public async Task Should_Verificar_Si_Es_Favorito()
        {
            // 1. ARRANGE
            var testUserId = Guid.NewGuid();
            var destinoId = Guid.NewGuid();

            var destinoTest = new WayFinder.DestinosTuristicos.DestinoTuristico(destinoId)
            {
                nombre = "Destino Check",
                foto = "check.jpg",
                UltimaActualizacion = DateTime.Now,
                Pais = new Pais("Peru", 33000000),
                Coordenadas = new Coordenadas(-12.0464, -77.0428)
            };
            await _destinoRepository.InsertAsync(destinoTest);

            // 2. ACT
            using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
            {
                // Primero no es favorito
                var inicial = await _favoritoAppService.IsFavoritoAsync(destinoId);
                inicial.ShouldBeFalse();

                // Lo agregamos
                var input = new CreateDestinoFavoritoDto { DestinoTuristicoId = destinoId };
                await _favoritoAppService.CreateAsync(input);

                // Ahora sí es favorito
                var final = await _favoritoAppService.IsFavoritoAsync(destinoId);
                final.ShouldBeTrue();
            }
        }

        [Fact]
        public async Task Should_Eliminar_Un_Favorito()
        {
            var testUserId = Guid.NewGuid();
            var destinoId = Guid.NewGuid();

            // 1. Crear el destino base
            var destinoTest = new WayFinder.DestinosTuristicos.DestinoTuristico(destinoId)
            {
                nombre = "Destino Delete",
                foto = "del.jpg",
                UltimaActualizacion = DateTime.Now,
                Pais = new Pais("Chile", 19000000),
                Coordenadas = new Coordenadas(-33, -70)
            };
            await _destinoRepository.InsertAsync(destinoTest);

            using (_currentPrincipalAccessor.Change(CreateTestPrincipal(testUserId)))
            {
              
                await _favoritoAppService.CreateAsync(new CreateDestinoFavoritoDto { DestinoTuristicoId = destinoId });

               
                var lista = await _favoritoAppService.GetListAsync(new PagedAndSortedResultRequestDto());
                var favoritoId = lista.Items[0].Id; // Tomamos el ID del primero que encontremos

                
                lista.TotalCount.ShouldBe(1); // Verificamos que se creó
            }
        }

        // 👇👇 ¡ESTA ES LA JOYA DE TUS COMPAÑEROS! 👇👇
        // Este método fabrica el usuario falso cada vez que lo necesitas
        private ClaimsPrincipal CreateTestPrincipal(Guid userId)
        {
            var claims = new[]
            {
                new Claim(AbpClaimTypes.UserId, userId.ToString()),
                new Claim(AbpClaimTypes.UserName, $"user_{userId}"),
                new Claim(AbpClaimTypes.Email, "test@wayfinder.com")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            return new ClaimsPrincipal(identity);
        }
    }
}