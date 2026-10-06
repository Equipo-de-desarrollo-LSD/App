using WayFinder.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace WayFinder.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class WayFinderController : AbpControllerBase
{
    protected WayFinderController()
    {
        LocalizationResource = typeof(WayFinderResource);
    }
}
