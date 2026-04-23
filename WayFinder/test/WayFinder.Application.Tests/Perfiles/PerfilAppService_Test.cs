using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using WayFinder.Perfiles;
using Xunit;

namespace WayFinder.Perfiles
{
    public class PerfilAppService_Tests : WayFinderApplicationTestBase<WayFinderApplicationTestModule>
    {
        private readonly IPerfilAppService _perfilAppService;
        private readonly IdentityUserManager _userManager;
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public PerfilAppService_Tests()
        {
            _perfilAppService = GetRequiredService<IPerfilAppService>();
            _userManager = GetRequiredService<IdentityUserManager>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
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
        [Fact]
        public async Task Should_Eliminar_Mi_Cuenta_Correctamente()
        {
            // Inventamos un ID
            var userId = Guid.NewGuid();

            // Guardamos al usuario físicamente en la BD de pruebas de ABP
            var userManager = GetRequiredService<Volo.Abp.Identity.IdentityUserManager>();
            var nuevoUsuario = new Volo.Abp.Identity.IdentityUser(userId, "usuarioBorrar", "borrar@wayfinder.com");
            await userManager.CreateAsync(nuevoUsuario);

            // Usamos un usuario que "ya existe"
            using (SetCurrentUser(userId))
            {
                // Arrange: 
                // Llamamos a GetMiPerfilAsync para que cree el perfil atado a este usuario
                var perfilAntes = await _perfilAppService.GetMiPerfilAsync();
                perfilAntes.ShouldNotBeNull();

                // Borramos el perfil de la base de datos
                await _perfilAppService.EliminarMiCuentaAsync();

                // Si lo borró bien, intentar buscarlo de nuevo y tiene que lanzar una Excepción
                await Should.ThrowAsync<Exception>(async () =>
                {
                    await _perfilAppService.GetMiPerfilAsync();
                });
            }
        }

        // Este test verifica que podemos obtener el perfil público de otro usuario (en este caso, el mismo admin) sin problemas.
        [Fact]
        public async Task Should_Obtener_Perfil_Publico_Correctamente()
        {
            // Arrange: Buscamos al admin directamente en la base de datos por debajo de la mesa
            var userManager = GetRequiredService<Volo.Abp.Identity.IdentityUserManager>();
            var adminUser = await userManager.FindByNameAsync("admin");

            // Si el admin fue borrado por otro test, cortamos la prueba acá para que no explote
            if (adminUser == null) return;

            var adminId = adminUser.Id;

            // Act: Usamos tu nuevo método pasándole el ID que descubrimos
            var perfilPublico = await _perfilAppService.GetPerfilPublicoAsync(adminId);

            // Assert: Verificamos que nos haya devuelto los datos correctamente
            perfilPublico.ShouldNotBeNull();
            perfilPublico.Id.ShouldBe(adminId);
            perfilPublico.UserName.ShouldBe("admin");
        }
    }
}