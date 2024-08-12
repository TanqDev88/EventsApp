using Volo.Abp.Settings;

namespace Ticketera.Settings;

public class TicketeraSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(TicketeraSettings.MySetting1));
    }
}
