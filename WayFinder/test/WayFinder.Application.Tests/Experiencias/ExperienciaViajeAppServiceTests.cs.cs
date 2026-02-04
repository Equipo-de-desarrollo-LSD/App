using System;
using System.Threading.Tasks;
using Shouldly; // Esto sirve para validar (Asserts)
using Volo.Abp.Domain.Repositories;
using WayFinder.DestinosTuristicos;
using Xunit;

namespace WayFinder.Experiencias
{
    // Heredamos de la clase base de Tests de ABP
    public class ExperienciaViajeAppServiceTests : WayFinderApplicationTestBase<WayFinderApplicationTestModule>
    {
        // Declaramos las variables que vamos a usar
        private readonly IExperienciaViajeAppService _experienciaService;
        private readonly IRepository<WayFinder.DestinosTuristicos.DestinoTuristico, Guid> _destinoRepository;

        public ExperienciaViajeAppServiceTests()
        {
            // El test "recupera" el servicio real para poder probarlo.
            _experienciaService = GetRequiredService<IExperienciaViajeAppService>();

            // También recuperamos el repositorio de destinos para crear datos de prueba
            _destinoRepository = GetRequiredService<IRepository<WayFinder.DestinosTuristicos.DestinoTuristico, Guid>>();
        }

        [Fact]
        public async Task Should_Create_A_Valid_Experiencia()
        {
            // --- ARRANGE (Preparar) ---
            // Creamos una ciudad falsa en la base de datos de memoria
            var destinoId = Guid.NewGuid();
            var destinoFalso = new WayFinder.DestinosTuristicos.DestinoTuristico(destinoId)
            {
                nombre = "Ciudad Test",
                foto = "foto_falsa.jpg", 
            };

            await _destinoRepository.InsertAsync(destinoFalso);

            var input = new CreateUpdateExperienciaViajeDto
            {
                DestinoTuristicoId = destinoId,
                Titulo = "Test Unitario Exitoso",
                Contenido = "Contenido de prueba para validar que todo funciona.",
                Sentimiento = TipoExperiencia.Positiva
            };

            // --- ACT (Actuar) ---
            // Probamos tu servicio real
            var result = await _experienciaService.CreateAsync(input);

            // --- ASSERT (Verificar) ---
            result.ShouldNotBeNull();
            result.Titulo.ShouldBe("Test Unitario Exitoso");
        }
        [Fact]
        public async Task Should_Update_Experiencia_Correctly()
        {
            // --- 1. ARRANGE (Preparar el escenario) ---
            // Primero, necesitamos un Destino y una Experiencia ya existente en la BD
            var destinoId = Guid.NewGuid();
            var destinoFalso = new WayFinder.DestinosTuristicos.DestinoTuristico(destinoId)
            {
                nombre = "Ciudad para Editar",
                foto = "foto.jpg"
            };
            await _destinoRepository.InsertAsync(destinoFalso);

            // Creamos la experiencia "Vieja"
            var experienciaInput = new CreateUpdateExperienciaViajeDto
            {
                DestinoTuristicoId = destinoId,
                Titulo = "Titulo Viejo",
                Contenido = "Contenido original.",
                Sentimiento = TipoExperiencia.Neutral
            };
            var experienciaCreada = await _experienciaService.CreateAsync(experienciaInput);

            // --- 2. ACT (Modificar) ---
            // Ahora simulamos que el usuario edita el título y el sentimiento
            var updateInput = new CreateUpdateExperienciaViajeDto
            {
                DestinoTuristicoId = destinoId, // El ID del destino se mantiene
                Titulo = "Titulo NUEVO y Mejorado", // <--- CAMBIO AQUÍ
                Contenido = "Contenido original.",
                Sentimiento = TipoExperiencia.Positiva // <--- CAMBIO AQUÍ
            };

            // Llamamos al servicio de Update usando el ID de la experiencia creada
            var resultadoActualizado = await _experienciaService.UpdateAsync(experienciaCreada.Id, updateInput);

            // --- 3. ASSERT (Verificar) ---
            // Verificamos que el objeto devuelto tenga los datos nuevos
            resultadoActualizado.Titulo.ShouldBe("Titulo NUEVO y Mejorado");
            resultadoActualizado.Sentimiento.ShouldBe(TipoExperiencia.Positiva);

            // (Opcional pero recomendado) Verificamos que en base de datos también cambió
            var experienciaEnDb = await _experienciaService.GetAsync(experienciaCreada.Id);
            experienciaEnDb.Titulo.ShouldBe("Titulo NUEVO y Mejorado");
        }
    }
}
