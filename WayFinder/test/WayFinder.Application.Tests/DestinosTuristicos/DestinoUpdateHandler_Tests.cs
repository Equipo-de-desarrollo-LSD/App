using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;
using WayFinder.DestinosTuristicosDTOs;
using WayFinder.EntityFrameworkCore;
using Xunit;

namespace WayFinder.DestinosTuristicos
{
    public class DestinoUpdateHandler_Tests : DestinoTurisiticoAppService_Tests<WayFinderApplicationTestModule>
    {

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IDbContextProvider<WayFinderDbContext> _dbContextProvider;
        private readonly IDestinoTuristicoAppService _destinoService;
        private readonly IListaSeguimientoAppService _listaSeguimientoAppService;

        public DestinoUpdateHandler_Tests()
        {
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
            _dbContextProvider = GetRequiredService<IDbContextProvider<WayFinderDbContext>>();
            _destinoService = GetRequiredService<IDestinoTuristicoAppService>();
            _listaSeguimientoAppService = GetRequiredService<IListaSeguimientoAppService>();
        }

        [Fact]
        public async Task UpdateAsync_ShouldTriggerNotification()
        {
            var userId = Guid.NewGuid();
            using (SetCurrentUser(userId))
            {
                using (var uow = _unitOfWorkManager.Begin())
                {
                    var dbContext = await _dbContextProvider.GetDbContextAsync();
                    var nuevoDestino = new GuardarDestinos
                    {
                        Nombre = "Cataratas VIP",
                        Foto = "c.jpg",
                        PaisNombre = "Argentina",
                        PaisPoblacion = 67000000,
                        CoordenadasLatitud = 48.8566,
                        CoordenadasLongitud = 2.3522,
                        UltimaActualizacion = DateTime.Now
                    };

                    var destinoCreado = await _destinoService.CreateAsync(nuevoDestino);
                    
                    await _listaSeguimientoAppService.SeguirDestinoAsync(destinoCreado.Id);                    
                    
                    await _destinoService.UpdateAsync(destinoCreado.Id, new GuardarDestinos
                    {
                        Nombre = "Cataratas VIP",
                        Foto = "cataratas.jpg",
                        PaisNombre = "Argentina",
                        UltimaActualizacion = DateTime.Now
                    });
                    
                    await uow.CompleteAsync();
                    var notification = await dbContext.Notificaciones
                        .AsNoTracking()
                        .FirstOrDefaultAsync(n => n.UsuarioId == userId);
                    Assert.NotNull(notification);
                    Assert.Contains("Cataratas VIP", notification.Mensaje);
                }
            }
        }

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
