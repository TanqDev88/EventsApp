using Ticketera.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Ticketera;

[DependsOn(
    typeof(TicketeraEntityFrameworkCoreTestModule)
    )]
public class TicketeraDomainTestModule : AbpModule
{

}
