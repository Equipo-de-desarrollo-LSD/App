using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;
using WayFinder.EntityFrameworkCore;
using WayFinder.Metricas;
using Xunit;

namespace WayFinder.Admin
{
    public class MetricaAppService_Tests : WayFinder.WayFinderTestBase<WayFinderApplicationTestModule>
    {
        private readonly IMetricaAppService _metricaAppService;
        private readonly IDbContextProvider<WayFinderDbContext> _dbContextProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public MetricaAppService_Tests()
        {
            _metricaAppService = GetRequiredService<IMetricaAppService>();
            _dbContextProvider = GetRequiredService<IDbContextProvider<WayFinderDbContext>>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
        }

        [Fact]
        public async Task Admin_Should_See_Api_Metrics()
        {
            var adminId = Guid.NewGuid();


            using (SetCurrentUser(adminId, "admin_user", new[] { "admin" }))
            {
                // 1. ARRANGE: Insertamos una métrica manual en la DB
                using (var uow = _unitOfWorkManager.Begin())
                {
                    var dbContext = await _dbContextProvider.GetDbContextAsync();
                    await dbContext.Set<MetricaApi>().AddAsync(
                        new MetricaApi(Guid.NewGuid(), "GeoDB", "/cities", 200, 150)
                    );
                    await uow.CompleteAsync();
                }

                // 2. ACT: Consultamos como Admin
                // Nota: En ABP Tests, por defecto se suele tener permisos de admin si no se restringe.
                var result = await _metricaAppService.GetListAsync();

                // 3. ASSERT
                Assert.NotEmpty(result);
                Assert.Equal("GeoDB", result[0].NombreServicio);
                Assert.True(result[0].TiempoRespuestaMs > 0);
            }
        }

        [Fact]
        public async Task Non_Admin_Should_Not_Access_Metrics()
        {
            // Simulamos un usuario que NO es admin
            using (SetCurrentUser(Guid.NewGuid()))
            {
                await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
                {
                    await _metricaAppService.GetListAsync();
                });
            }
        }

        protected virtual IDisposable SetCurrentUser(Guid? userId, string userName = "test_user", string[] roles = null)
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
                // AGREGAR ESTA LÓGICA: Si se pasan roles, los agregamos como claims
                if (roles != null)
                {
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(AbpClaimTypes.Role, role));
                    }
                }
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
