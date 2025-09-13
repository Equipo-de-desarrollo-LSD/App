using Volo.Abp.Modularity;

namespace WayFinder;

[DependsOn(
    typeof(WayFinderApplicationModule),
    typeof(WayFinderDomainTestModule)
)]
public class WayFinderApplicationTestModule : AbpModule
{

}
