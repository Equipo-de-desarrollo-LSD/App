using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper; 
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using WayFinder.Calificaciones;
using WayFinder.DestinosTuristicosDTOs;

namespace WayFinder;

[DependsOn(
    typeof(WayFinderDomainModule),
    typeof(WayFinderApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule)
    )]
public class WayFinderApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //context.Services.AddAutoMapperObjectMapper<WayFinderApplicationModule>();
        

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<WayFinderApplicationModule>();
        });

        context.Services.AddTransient<IBuscarCiudadService, GeoDbBuscarCiudadService>();
    }
}
