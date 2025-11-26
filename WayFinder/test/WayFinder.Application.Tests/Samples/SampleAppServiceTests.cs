﻿using Shouldly;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Xunit;

namespace WayFinder.Samples;

/* This is just an example test class.
 * Normally, you don't test code of the modules you are using
 * (like IIdentityUserAppService here).
 * Only test your own application services.
 */
public abstract class SampleAppServiceTests<TStartupModule> : WayFinderApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IIdentityUserAppService _userAppService;
    private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;  // <-- Esta línea debe estar aquí

    protected SampleAppServiceTests()
    {
        _userAppService = GetRequiredService<IIdentityUserAppService>();
        _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();  // <-- Esta línea también
    }

    [Fact]
    public async Task Initial_Data_Should_Contain_Admin_User()
    {
        // Obtener el servicio ICurrentUser para suplantar la identidad
        // Usar WithUnitOfWorkAsync para asegurar un contexto de transacción
        await WithUnitOfWorkAsync(async () =>
        {
            // Simular un usuario autenticado (admin)
            var adminUserId = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");

            var claims = new[]
            {
                new Claim(AbpClaimTypes.UserId, adminUserId.ToString()),
                new Claim(AbpClaimTypes.UserName, "admin"),
                new Claim(AbpClaimTypes.Role, "admin")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            // IMPORTANTE: Cambiar temporalmente el contexto para simular un usuario "admin"
            using (_currentPrincipalAccessor.Change(principal))
            {
                //Act
                var result = await _userAppService.GetListAsync(new GetIdentityUsersInput());

                //Assert
                result.TotalCount.ShouldBeGreaterThan(0);
                result.Items.ShouldContain(u => u.UserName == "admin");
            }
        });
    }
}
