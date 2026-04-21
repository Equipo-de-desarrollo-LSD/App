using Autofac.Core;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Polly.Caching;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Volo.Abp.Validation;
using WayFinder.DestinosTuristicos;
using WayFinder.DestinosTuristicosDTOs;
using Xunit;


namespace WayFinder.Calificacion
{
    // Obtenemos las dependencias necesarias para las pruebas unitarias
    // Usamos WayFinderTestBase para aprovechar la configuración y servicios comunes
    public class CalificacionAppService_Test: WayFinder.WayFinderTestBase<WayFinderApplicationTestModule>
    {
        // Servicio que vamos a probar
        private readonly IDestinoTuristicoAppService _services;
        private readonly ICalificacionAppService _calificacionAppService; 
        // private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

       public CalificacionAppService_Test()
        {
            // Utilizamos el contenedor de dependencias para obtener una instancia del servicio que queremos probar.
            // Esto asegura que todas las dependencias se resuelvan correctamente.
            // Algo similar hicimos en DestinoTurisiticoAppService_Tests
            _services = GetRequiredService<IDestinoTuristicoAppService>();
            _calificacionAppService = GetRequiredService<ICalificacionAppService>();
            // _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
        }
        [Fact]
        // Test para verificar que la calificación se asocia correctamente con el usuario actual
        public async Task CalificarDestinoAsync_Should_Associate_Puntaje_With_Current_User()
        {
            // 1. Inyectamos el servicio de destinos y de usuarios
            var destinoAppService = GetRequiredService<IDestinoTuristicoAppService>();
            var userManager = GetRequiredService<Volo.Abp.Identity.IdentityUserManager>();

            // Creamos el destino de prueba
            var nuevoDestino = new GuardarDestinos
            {
                Nombre = "Playa Paraíso",
                Foto = "playa_paraiso.jpg", // ← necesario
                PaisNombre = "España",
                PaisPoblacion = 49000000,
                CoordenadasLatitud = 36.7213,
                CoordenadasLongitud = -4.4214,
                UltimaActualizacion = DateTime.Now
            };

            var destino = await destinoAppService.CreateAsync(nuevoDestino);

            // Creamos el usuario físicamente en la BD
            var userId = Guid.NewGuid();
            var nuevoUsuario = new Volo.Abp.Identity.IdentityUser(userId, "usuarioPrueba", "prueba@wayfinder.com");
            await userManager.CreateAsync(nuevoUsuario);

            // Simulamos ser el usuario logueado
            using (SetCurrentUser(userId))
            {
                var input = new CrearCalificacionDto
                {
                    // Usamos el ID del destino que acabamos de crear con éxito
                    UserId = userId,
                    DestinoId = destino.Id,
                    Puntaje = 5,
                    Comentario = "Excelente destino turístico"
                };

                // Act: Creamos la calificación
                var result = await _calificacionAppService.CreateAsync(input);

                // Assert: Verificamos que todo guardó bien
                result.ShouldNotBeNull();
                result.Puntaje.ShouldBe(5);
                result.Comentario.ShouldBe("Excelente destino turístico");
                result.DestinoId.ShouldBe(destino.Id);
                result.UserId.ShouldBe(userId);
                Assert.Equal(userId, result.UserId);
            }
        }


        [Fact]
        // Test para verificar que se lanza una excepción si el usuario no está autenticado al calificar un destino
        public async Task CalificarDestinoAsync_Should_Throw_If_Not_Autenticated()
        {
            // Arrange
            var input = new CrearCalificacionDto
            {
                DestinoId = Guid.NewGuid(),
                Puntaje = 5,
            };
            // Act & Assert
            await Should.ThrowAsync<AbpAuthorizationException>(async () =>
            {
                await _calificacionAppService.CreateAsync(input);
            });
        }
        [Fact]
        // Test para verificar que se lanza una excepción si el puntaje es inválido al calificar un destino
        public async Task CalificarDestinoAsync_Should_Throw_If_Puntaje_Invalid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            using (SetCurrentUser(userId))
            {
                var input = new CrearCalificacionDto
                {
                    DestinoId = Guid.NewGuid(),
                    Puntaje = 6, // Puntaje inválido
                };
                // Act & Assert
                AbpValidationException abpValidationException = await Should.ThrowAsync<AbpValidationException>(async () =>
                {
                    await _calificacionAppService.CreateAsync(input);
                });
            }
        }
        [Fact]
        // Test Req 5.3: Verificar que un usuario PUEDE borrar su propia calificación
        public async Task DeleteAsync_Should_Delete_Own_Calificacion()
        {
            // Arrange
            var destinoAppService = GetRequiredService<IDestinoTuristicoAppService>();
            var userManager = GetRequiredService<Volo.Abp.Identity.IdentityUserManager>();

            var destino = await destinoAppService.CreateAsync(new GuardarDestinos
            {
                Nombre = "Playa Borrable",
                Foto = "test.jpg",
                PaisNombre = "Test",
                PaisPoblacion = 100,
                CoordenadasLatitud = 0,
                CoordenadasLongitud = 0,
                UltimaActualizacion = DateTime.Now
            });

            var userId = Guid.NewGuid();
            await userManager.CreateAsync(new Volo.Abp.Identity.IdentityUser(userId, "usuarioBorrador", "borrador@wayfinder.com"));

            Guid calificacionId;

            using (SetCurrentUser(userId))
            {
                var input = new CrearCalificacionDto { DestinoId = destino.Id, Puntaje = 5, Comentario = "Para borrar" };
                var result = await _calificacionAppService.CreateAsync(input);
                calificacionId = result.Id; // Guardamos el ID generado

                // Act: El mismo usuario intenta borrarla
                await _calificacionAppService.DeleteAsync(calificacionId);
            }

            // Assert: Verificamos que la lista del destino quedó vacía
            var lista = await _calificacionAppService.GetCalificacionesPorDestinoAsync(destino.Id);
            lista.ShouldBeEmpty();
        }
       
        [Fact]
        public async Task DeleteAsync_Should_Throw_If_Not_Owner()
        {
            // Arrange
            var destinoAppService = GetRequiredService<IDestinoTuristicoAppService>();
            var userManager = GetRequiredService<Volo.Abp.Identity.IdentityUserManager>();

            var destino = await destinoAppService.CreateAsync(new GuardarDestinos
            {
                Nombre = "Playa Intocable",
                Foto = "test.jpg",
                PaisNombre = "Test",
                PaisPoblacion = 100,
                CoordenadasLatitud = 0,
                CoordenadasLongitud = 0,
                UltimaActualizacion = DateTime.Now
            });

            var userA = Guid.NewGuid();
            var userB = Guid.NewGuid();
            // Importante: Crear los usuarios en el IdentityManager para que existan en la BD del test
            await userManager.CreateAsync(new Volo.Abp.Identity.IdentityUser(userA, "UserA", "usera@wayfinder.com"));
            await userManager.CreateAsync(new Volo.Abp.Identity.IdentityUser(userB, "UserB", "userb@wayfinder.com"));

            Guid calificacionId;

            // User A crea la calificación
            using (SetCurrentUser(userA))
            {
                var result = await _calificacionAppService.CreateAsync(new CrearCalificacionDto
                {
                    DestinoId = destino.Id,
                    Puntaje = 5,
                    UserId = userA // Asegúrate que tu DTO o servicio asigne esto
                });
                calificacionId = result.Id;
            }

            // Act & Assert: User B intenta borrarla
            using (SetCurrentUser(userB))
            {
                // Si tu AppService no tiene un "if (calificacion.UserId != CurrentUser.Id) throw...", 
                // este test seguirá fallando.
                await Should.ThrowAsync<Volo.Abp.Domain.Entities.EntityNotFoundException>(async () =>
                {
                    await _calificacionAppService.DeleteAsync(calificacionId);
                });
            }
        }
        [Fact]
        // Test extra: El promedio debe ser 0 si nadie calificó el destino aún
        public async Task GetPromedioAsync_Should_Return_Zero_If_No_Ratings()
        {
            // Arrange: Un ID de destino inventado que sabemos que no tiene calificaciones
            var destinoVacioId = Guid.NewGuid();

            // Act
            var promedio = await _calificacionAppService.GetPromedioAsync(destinoVacioId);

            // Assert
            promedio.ShouldBe(0.0);
        }
        // Método auxiliar para establecer el usuario actual en el contexto de seguridad
        // Esto es crucial para simular la autenticación en las pruebas unitarias.
        protected virtual IDisposable SetCurrentUser(Guid? userId, string userName = "test_user")
        {
            // Resolvemos el servicio de ABP que maneja la identidad del usuario en el hilo actual.
            var currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();

            // 1. Crear una identidad con los Claims (declaraciones) necesarios.
            var claims = new List<Claim>();
            if (userId.HasValue)
            {
                // Usamos AbpClaimTypes.UserId para el ID del usuario (clave para ICurrentUser)
                claims.Add(new Claim(AbpClaimTypes.UserId, userId.Value.ToString()));
                // Agregamos un nombre de usuario (opcional, pero buena práctica)
                claims.Add(new Claim(AbpClaimTypes.UserName, userName));
                // claims.Add(new Claim(ClaimTypes.Role, "admin")); // Ejemplo si necesitaras roles
            }

            // 2. Crear el ClaimsPrincipal
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            // 3. Cambiar el contexto de seguridad.
            // .Change(principal) devuelve un IDisposable, que es clave.
            return currentPrincipalAccessor.Change(principal);
        }
    }   
}
