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
using WayFinder.DestinosTuristicos;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace WayFinder.DestinosTuristicos
{
    public class ListaSeguimientoAppService_Tests : WayFinder.WayFinderTestBase<WayFinderApplicationTestModule>
    {

        private readonly IListaSeguimientoAppService _listaSeguimientoAppService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IDestinoTuristicoAppService _destinoTuristicoAppService;
        private readonly IDbContextProvider<WayFinderDbContext> _dbContextProvider;

        public ListaSeguimientoAppService_Tests()
        {
            _listaSeguimientoAppService = GetRequiredService<IListaSeguimientoAppService>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
            _destinoTuristicoAppService = GetRequiredService<IDestinoTuristicoAppService>();
            _dbContextProvider = GetRequiredService<IDbContextProvider<WayFinderDbContext>>();
        }

        [Fact]
        public async Task SeguirDestinoAsync_ShouldPersistRelationship()
        {
            var userId = Guid.NewGuid();
            using (SetCurrentUser(userId))
            {
                using (var uow = _unitOfWorkManager.Begin())
                {
                    var nuevoDestino = new GuardarDestinos
                    {
                        Nombre = "Paris",
                        Foto = "paris.jpg",
                        PaisNombre = "Francia",
                        PaisPoblacion = 67000000,
                        CoordenadasLatitud = 48.8566,
                        CoordenadasLongitud = 2.3522,
                        UltimaActualizacion = DateTime.Now
                    };

                    // Creamos el destino
                    var destinoCreado = await _destinoTuristicoAppService.CreateAsync(nuevoDestino);


                    await _listaSeguimientoAppService.SeguirDestinoAsync(destinoCreado.Id);

                    var dbContext = await _dbContextProvider.GetDbContextAsync();
                    var seguimiento =  await dbContext.ListasSeguimiento
                        .AsNoTracking() // Recomendado para evitar problemas de caché en el test
                        .FirstOrDefaultAsync(x => x.DestinoId == destinoCreado.Id && x.UsuarioId == userId);

                    await uow.CompleteAsync();

                    Assert.NotNull(seguimiento);
                    Assert.Equal(destinoCreado.Id, seguimiento.DestinoId);
                    Assert.Equal(userId, seguimiento.UsuarioId);
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
