using Volo.Abp.Modularity;

namespace WayFinder;

public abstract class WayFinderApplicationTestBase<TStartupModule> : WayFinderTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
