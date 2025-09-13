using WayFinder.Localization;
using Volo.Abp.Application.Services;

namespace WayFinder;

/* Inherit your application services from this class.
 */
public abstract class WayFinderAppService : ApplicationService
{
    protected WayFinderAppService()
    {
        LocalizationResource = typeof(WayFinderResource);
    }
}
