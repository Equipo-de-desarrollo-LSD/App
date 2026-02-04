using System;
using System.Threading.Tasks;
using Shouldly; // Librería favorita de ABP para aserciones (Asserts limpios)
using Xunit;
using WayFinder.DestinosTuristicos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using WayFinder;

namespace WayFinder.Favoritos
{
    // Heredamos de TestBase para tener la DB en memoria y la inyección de dependencias lista
    public class DestinoFavoritoAppServiceTests : WayFinderApplicationTestBase<WayFinderApplicationTestModule>
    {
        private readonly IDestinoFavoritoAppService _favoritoAppService;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;

        public DestinoFavoritoAppServiceTests()
        {
            // Inyectamos los servicios que vamos a probar y usar
            _favoritoAppService = GetRequiredService<IDestinoFavoritoAppService>();
            _destinoRepository = GetRequiredService<IRepository<DestinoTuristico, Guid>>();
        }

        [Fact]
        public async Task Should_Agregar_Y_Listar_Favorito()
        {
            // 1. ARRANGE (Preparar el escenario)
            // Primero necesitamos que exista un destino "real" en la DB para poder darle like.
            var destinoId = Guid.NewGuid();

            // IMPORTANTE: Ajusta este constructor si tu entidad pide más datos o datos distintos.
            // Asumo un constructor básico y asignación de propiedades.
            var destinoTest = new DestinoTuristico(destinoId)
            {
                nombre = "Playa Test",
                foto = "test.jpg",
                UltimaActualizacion = DateTime.Now,

                // 👇 AGREGA ESTO: Inicializa el objeto Pais
                Pais = new Pais("Argentina", 45000000),

                // 👇 AGREGA ESTO TAMBIÉN (Seguro también es obligatorio)
               Coordenadas = new Coordenadas(-34.6037, -58.3816)
            };

            // Si tu entidad usa ValueObjects para Pais/Coordenadas, quizás debas inicializarlos así:
            // destinoTest.Pais = new Pais("Argentina", 45000000); 

            await _destinoRepository.InsertAsync(destinoTest);

            // 2. ACT (Ejecutar la acción a probar)
            var input = new CreateDestinoFavoritoDto { DestinoTuristicoId = destinoId };
            await _favoritoAppService.CreateAsync(input);

            // 3. ASSERT (Verificar el resultado)
            // Pedimos la lista de favoritos
            var resultado = await _favoritoAppService.GetListAsync(new PagedAndSortedResultRequestDto());

            // Validaciones
            resultado.TotalCount.ShouldBe(1); // Debería haber 1 elemento
            resultado.Items[0].DestinoTuristicoId.ShouldBe(destinoId); // El ID debe coincidir
            resultado.Items[0].NombreDestino.ShouldBe("Playa Test"); // ¡Magia! Debe traer el nombre del otro repo
        }
        [Fact]
        public async Task Should_Eliminar_Un_Favorito()
        {
            // 1. ARRANGE: Preparamos el escenario (Creamos un destino y lo hacemos favorito)
            var destinoId = Guid.NewGuid();
            var destinoTest = new DestinoTuristico(destinoId)
            {
                nombre = "Destino Borrable",
                foto = "delete.jpg",
                UltimaActualizacion = DateTime.Now,
                Pais = new Pais("Chile", 19000000),
                Coordenadas = new Coordenadas(-33.4489, -70.6693)
            };
            await _destinoRepository.InsertAsync(destinoTest);

            // Creamos el favorito
            var input = new CreateDestinoFavoritoDto { DestinoTuristicoId = destinoId };
            await _favoritoAppService.CreateAsync(input);

            // 2. ACT: Lo eliminamos
            // (Asegúrate de tener un método DeleteAsync en tu AppService, o usa el ID del favorito creado)
            await _favoritoAppService.DeleteByDestinoIdAsync(destinoId);

            // 3. ASSERT: Verificamos que ya no esté
            var resultado = await _favoritoAppService.GetListAsync(new PagedAndSortedResultRequestDto());

            resultado.TotalCount.ShouldBe(0); // La lista debería estar vacía
        }
        [Fact]
        public async Task Should_Verificar_Si_Es_Favorito()
        {
            // 1. ARRANGE: Crear un destino
            var destinoId = Guid.NewGuid();
            var destinoTest = new DestinoTuristico(destinoId)
            {
                nombre = "Destino Check",
                foto = "check.jpg",
                UltimaActualizacion = DateTime.Now,
                Pais = new Pais("Peru", 33000000),
                Coordenadas = new Coordenadas(-12.0464, -77.0428)
            };
            await _destinoRepository.InsertAsync(destinoTest);

            // 2. ACT & ASSERT (Parte 1): Al principio NO debe ser favorito
            var esFavoritoInicial = await _favoritoAppService.IsFavoritoAsync(destinoId);
            esFavoritoInicial.ShouldBeFalse();

            // 3. ACT (Parte 2): Lo agregamos a favoritos
            await _favoritoAppService.CreateAsync(new CreateDestinoFavoritoDto { DestinoTuristicoId = destinoId });

            // 4. ASSERT (Parte 3): Ahora SÍ debe ser favorito
            var esFavoritoFinal = await _favoritoAppService.IsFavoritoAsync(destinoId);
            esFavoritoFinal.ShouldBeTrue();
        }
        [Fact]
        public async Task Should_Not_Duplicar_Favorito()
        {
            // 1. ARRANGE
            var destinoId = Guid.NewGuid();
            var destinoTest = new DestinoTuristico(destinoId)
            {
                nombre = "Destino Único",
                foto = "unique.jpg",
                UltimaActualizacion = DateTime.Now,
                Pais = new Pais("Brasil", 214000000),
                Coordenadas = new Coordenadas(-14.2350, -51.9253)
            };
            await _destinoRepository.InsertAsync(destinoTest);

            var input = new CreateDestinoFavoritoDto { DestinoTuristicoId = destinoId };

            // 2. ACT: Intentamos agregarlo DOS veces
            await _favoritoAppService.CreateAsync(input);
            await _favoritoAppService.CreateAsync(input); // Segunda llamada (intencional)

            // 3. ASSERT: Solo debería haber 1 registro en la lista, no 2
            var resultado = await _favoritoAppService.GetListAsync(new PagedAndSortedResultRequestDto());

            resultado.TotalCount.ShouldBe(1);
            // Si sale 2, significa que tu AppService no está controlando duplicados.
        }
    }

}