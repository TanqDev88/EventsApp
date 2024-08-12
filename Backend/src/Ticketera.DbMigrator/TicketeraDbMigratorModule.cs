using Ticketera.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Ticketera.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(TicketeraEntityFrameworkCoreModule),
    typeof(TicketeraApplicationContractsModule)
    )]
public class TicketeraDbMigratorModule : AbpModule
{
}
