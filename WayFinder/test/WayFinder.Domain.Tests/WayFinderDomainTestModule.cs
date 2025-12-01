using Volo.Abp.Modularity;

namespace WayFinder;

[DependsOn(
    typeof(WayFinderDomainModule),
    typeof(WayfinderTestBaseModule)
)]
public class WayFinderDomainTestModule : AbpModule
{

}
