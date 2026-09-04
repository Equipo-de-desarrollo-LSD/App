using Volo.Abp.Settings;

namespace WayFinder.Settings;

public class WayFinderSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(WayFinderSettings.MySetting1));
    }
}
