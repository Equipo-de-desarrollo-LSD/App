using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper; 
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using WayFinder.DestinosTuristicos;
using WayFinder.DestinosTuristicosDTOs;

namespace WayFinder;

[DependsOn(
    typeof(WayFinderDomainModule),
    typeof(WayFinderApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
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
/*
 public override void ConfigureServices(ServiceConfigurationContext context)
{
    // ... (otra configuración) ...

    // Esta línea es suficiente para que el nuevo servicio funcione
    context.Services.AddHttpClient(); 

    // ... (otra configuración) ...
}

namespace TravelBuddy;

[DependsOn(
    typeof(TravelBuddyDomainModule),
    typeof(TravelBuddyApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class TravelBuddyApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<TravelBuddyApplicationModule>();
        });

        // Registro de GeoDbCitySearchService como implementación de ICitySearchService
        context.Services.AddTransient<ICitySearchService, GeoDbCitySearchService>();
    }
}
 */