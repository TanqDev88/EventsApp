using System;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Uow;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using EFCoreSecondLevelCacheInterceptor;

namespace Ticketera.EntityFrameworkCore;

[DependsOn(
    typeof(TicketeraDomainModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule)
    )]
public class TicketeraEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        TicketeraEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<TicketeraDbContext>(options =>
        {
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        var realTablesNames = new string[] { 
            "AbpUsers",             
            "EventDates", 
            "Events", 
            "FileAttachments",             
            "ProviderPayments", 
            "TicketCategories", 
            "Tickets", 
            "TicketSectors", 
            "UserEvent" 
        };
        context.Services.AddEFSecondLevelCache(options =>
                options.UseMemoryCacheProvider()
                       .DisableLogging(true)
                       .CacheAllQueries(CacheExpirationMode.Sliding, TimeSpan.MaxValue)
                       //.CacheQueriesContainingTableNames(CacheExpirationMode.Sliding, TimeSpan.MaxValue, realTableNames: realTablesNames)
                       .UseCacheKeyPrefix("EF_"));

        Configure<AbpDbContextOptions>(options =>
        {
            /* The main point to change your DBMS.
             * See also TicketeraMigrationsDbContextFactory for EF Core tooling. */
            //options.UseSqlServer();
            options.Configure(config =>
            {
                var interceptor = config.ServiceProvider.GetRequiredService<SecondLevelCacheInterceptor>();
                config.UseSqlServer().AddInterceptors(interceptor);
            });
        });

    }
}
