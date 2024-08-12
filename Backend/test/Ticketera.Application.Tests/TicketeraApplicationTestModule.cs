using Volo.Abp.Modularity;

namespace Ticketera;

[DependsOn(
    typeof(TicketeraApplicationModule),
    typeof(TicketeraDomainTestModule)
    )]
public class TicketeraApplicationTestModule : AbpModule
{

}
