using Volo.Abp.Modularity;
using WayFinder.EntityFrameworkCore; 

namespace WayFinder;

[DependsOn(
    typeof(WayFinderApplicationModule),
    typeof(WayFinderDomainTestModule),
    typeof(WayFinderEntityFrameworkCoreTestModule)
)]
public class WayFinderApplicationTestModule : AbpModule
{

}
