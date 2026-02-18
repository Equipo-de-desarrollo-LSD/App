using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Xunit;
using WayFinder.Perfiles;
using Volo.Abp.Identity;
using System;

namespace WayFinder.Perfiles
{
    public class PerfilAppService_Tests : WayFinderApplicationTestBase<WayFinderApplicationTestModule>
    {
        private readonly IPerfilAppService _perfilAppService;
        private readonly IdentityUserManager _userManager;
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

        public PerfilAppService_Tests()
        {
            _perfilAppService = GetRequiredService<IPerfilAppService>();
            _userManager = GetRequiredService<IdentityUserManager>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
        }

        // Este método es el "truco" para simular que estamos logueados como admin.
        private async Task<ClaimsPrincipal> GetAdminPrincipalAsync()
        {
            var adminUser = await _userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                throw new Exception("¡El usuario admin no existe en la base de datos de prueba!");
            }

            var claims = new[]
            {
                new Claim(AbpClaimTypes.UserId, adminUser.Id.ToString()),
                new Claim(AbpClaimTypes.UserName, adminUser.UserName),
                new Claim(AbpClaimTypes.Email, adminUser.Email)
            };

            var identity = new ClaimsIdentity(claims, "Test");
            return new ClaimsPrincipal(identity);
        }

        [Fact]
        public async Task Should_Obtener_Perfil_Inicial_Vacio()
        {
            // Arrange: Primero, obtenemos un ClaimsPrincipal que representa al usuario admin.
            var principal = await GetAdminPrincipalAsync();

            using (_currentPrincipalAccessor.Change(principal))
            {
                // Act: Ahora sí, somos Admin mientras estemos dentro de estas llaves { }
                var result = await _perfilAppService.GetMiPerfilAsync();

                // Assert
                result.ShouldNotBeNull();
                result.UserName.ShouldBe("admin");
                result.Foto.ShouldBeNull();
            }
        }

        // Este test verifica que podemos actualizar el perfil del usuario admin correctamente.
        [Fact]
        public async Task Should_Actualizar_Perfil_Correctamente()
        {
            var principal = await GetAdminPrincipalAsync();

            using (_currentPrincipalAccessor.Change(principal))
            {
                // Arrange
                var input = new ActualizarPerfilDto
                {
                    Nombre = "Valentina",
                    Apellido = "Test",
                    Email = "admin@abp.io",
                    Foto = "foto-test.jpg",
                    Preferencias = "Me gusta hacer Testing"
                };

                // Act
                await _perfilAppService.UpdateMiPerfilAsync(input);

                // Assert
                var perfilActualizado = await _perfilAppService.GetMiPerfilAsync();

                perfilActualizado.Nombre.ShouldBe("Valentina");
                perfilActualizado.Foto.ShouldBe("foto-test.jpg");
                perfilActualizado.Preferencias.ShouldBe("Me gusta hacer Testing");
            }
        }
        // Este test verifica que si actualizamos el perfil del usuario admin dos veces,
        // la segunda vez se actualizan los datos existentes (en vez de crear un nuevo registro).

        [Fact]
        public async Task Should_Modificar_Perfil_Ya_Existente()
        {
            var principal = await GetAdminPrincipalAsync();

            using (_currentPrincipalAccessor.Change(principal))
            {
                // 1. Primera actualización (Esto ejecuta el INSERT)
                await _perfilAppService.UpdateMiPerfilAsync(new ActualizarPerfilDto
                {
                    Nombre = "Valentina",
                    Apellido = "Original",
                    Email = "admin@abp.io",
                    Foto = "foto-v1.jpg",
                    Preferencias = "Gustos V1"
                });

                // 2. Segunda actualización (Esto debería ejecutar el UPDATE - el 'else')
                var inputModificado = new ActualizarPerfilDto
                {
                    Nombre = "Valentina",
                    Apellido = "Cambio",      // Cambiamos el apellido
                    Email = "admin@abp.io",
                    Foto = "foto-v2.jpg",     // Cambiamos la foto
                    Preferencias = "Gustos V2" // Cambiamos gustos
                };

                await _perfilAppService.UpdateMiPerfilAsync(inputModificado);

                // 3. Verificamos que se haya quedado con los DATOS NUEVOS
                var perfilFinal = await _perfilAppService.GetMiPerfilAsync();

                perfilFinal.Apellido.ShouldBe("Cambio");
                perfilFinal.Foto.ShouldBe("foto-v2.jpg");
                perfilFinal.Preferencias.ShouldBe("Gustos V2");
            }
        }
        // Este test verifica que al eliminar la cuenta del usuario admin, su perfil ya no se pueda obtener 
        public async Task Should_Eliminar_Mi_Cuenta_Correctamente()
        {
            // Arrange: 
            // En ABP, las pruebas corren automáticamente bajo un usuario "Admin" simulado.
            // Primero, nos aseguramos de que el perfil exista (podemos llamar a GetMiPerfilAsync para confirmar que no explota antes de borrar)
            var perfilAntes = await _perfilAppService.GetMiPerfilAsync();
            perfilAntes.ShouldNotBeNull();

            // Act: 
            // ¡Apretamos el botón rojo!
            await _perfilAppService.EliminarMiCuentaAsync();

            // Assert: 
            // Si el usuario se eliminó correctamente, intentar buscar su perfil nuevamente 
            // debería lanzar una excepción (porque _userManager.GetByIdAsync no lo va a encontrar).
            await Should.ThrowAsync<Exception>(async () =>
            {
                await _perfilAppService.GetMiPerfilAsync();
            });
        }
    }
}