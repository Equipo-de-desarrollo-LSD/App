using Volo.Abp.Modularity;

namespace WayFinder;

[DependsOn(
    typeof(WayFinderDomainModule),
    typeof(WayFinderTestBaseModule)
)]
public class WayFinderDomainTestModule : AbpModule
{

}
