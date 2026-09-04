using WayFinder.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace WayFinder.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(WayFinderEntityFrameworkCoreModule),
    typeof(WayFinderApplicationContractsModule)
)]
public class WayFinderDbMigratorModule : AbpModule
{
}
