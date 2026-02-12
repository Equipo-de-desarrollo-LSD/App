using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using WayFinder.EntityFrameworkCore;
using WayFinder.NotificacionesDTOs;
using Xunit;

namespace WayFinder.Notificaciones
{
    public class NotificacionAppService_Tests : WayFinder.WayFinderTestBase<WayFinderApplicationTestModule>
    {
        private readonly INotificacionAppService _notificacionAppService;
        private readonly IDbContextProvider<WayFinderDbContext> _dbContextProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public NotificacionAppService_Tests()
        {
            _notificacionAppService = GetRequiredService<INotificacionAppService>();
            _dbContextProvider = GetRequiredService<IDbContextProvider<WayFinderDbContext>>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
        }

        [Fact]
        public async Task MarcarComoLeidaAsync_ShouldUpdateDatabase()
        {
            // Creamos un ID de usuario simulado
            var userId = Guid.NewGuid();
            using (SetCurrentUser(userId))
            {
                using (var uow = _unitOfWorkManager.Begin())
                {
                    var id = Guid.NewGuid();
                    var dbContext = await _dbContextProvider.GetDbContextAsync();

                    var notificacion = new Notificacion(
                        id,
                        userId, // Usamos el mismo ID fijo
                        Guid.NewGuid(),
                        "Cambio en Destino",
                        "Se ha actualizado la información"
                    );

                    await dbContext.Notificaciones.AddAsync(notificacion);
                    await uow.CompleteAsync();

                    await _notificacionAppService.MarcarComoLeidaAsync(id);

                    var updatedNotif = await dbContext.Notificaciones
                        .AsNoTracking() // Recomendado para evitar problemas de caché en el test
                        .FirstOrDefaultAsync(n => n.Id == id);

                    Assert.NotNull(updatedNotif);
                    Assert.True(updatedNotif.Leido);
                    
                }
            }
        }

        [Fact]
        public async Task MarcarComoNoLeidaAsync_ShouldUpdateDatabase()
        {
            // Creamos un ID de usuario simulado
            var userId = Guid.NewGuid();
            using (SetCurrentUser(userId))
            {
                using (var uow = _unitOfWorkManager.Begin())
                {
                    var id = Guid.NewGuid();
                    var dbContext = await _dbContextProvider.GetDbContextAsync();

                    var notificacion = new Notificacion(
                        id,
                        userId, // Usamos el mismo ID fijo
                        Guid.NewGuid(),
                        "Cambio en Destino",
                        "Se ha actualizado la información"
                    );
                    notificacion.Leido = true; // Inicialmente marcada como leída

                    await dbContext.Notificaciones.AddAsync(notificacion);
                    await uow.CompleteAsync();
                    // Act
                    await _notificacionAppService.MarcarComoNoLeidaAsync(id);

                    // Assert

                    //var updatedNotif = await dbContext.Notificaciones.FindAsync(id);
                    // Assert: SOLUCIÓN -> Usar AsNoTracking para saltar la caché de memoria
                    var updatedNotif = await dbContext.Notificaciones
                        .AsNoTracking() // Esto obliga a leer el valor real de la DB
                        .FirstOrDefaultAsync(n => n.Id == id);

                    Assert.NotNull(updatedNotif);
                    Assert.False(updatedNotif.Leido);

                }
            }
        }

        [Fact]
        public async Task MarcarComoLeida_ShouldThrowExceptionIfNotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                await _notificacionAppService.MarcarComoLeidaAsync(Guid.NewGuid());
            });
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