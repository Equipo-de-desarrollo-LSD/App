using Microsoft.Extensions.Localization;
using WayFinder.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace WayFinder;

[Dependency(ReplaceServices = true)]
public class WayFinderBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<WayFinderResource> _localizer;

    public WayFinderBrandingProvider(IStringLocalizer<WayFinderResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
