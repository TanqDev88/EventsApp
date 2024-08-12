using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Ticketera;

[Dependency(ReplaceServices = true)]
public class TicketeraBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "Ticketera";
}
