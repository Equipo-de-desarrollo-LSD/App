using Volo.Abp.Modularity;

namespace WayFinder;

/* Inherit from this class for your domain layer tests. */
public abstract class WayFinderDomainTestBase<TStartupModule> : WayFinderTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
