using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper; 
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using WayFinder.Calificaciones;
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
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule)
    )]
public class WayFinderApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //context.Services.AddAutoMapperObjectMapper<WayFinderApplicationModule>();
        context.Services.AddHttpClient<IEventosService, TicketMasterEventosService>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<WayFinderApplicationModule>();
        });

        context.Services.AddTransient<IBuscarCiudadService, GeoDbBuscarCiudadService>();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        // Registramos el worker para sincronizar eventos en segundo plano
        context.AddBackgroundWorkerAsync<WayFinder.Workers.TicketMasterEventosWorker>();
    }
}
