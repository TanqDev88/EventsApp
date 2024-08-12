using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Imaging;

namespace Ticketera;

[DependsOn(
    typeof(TicketeraDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(TicketeraApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
[DependsOn(typeof(AbpImagingAbstractionsModule))]
    [DependsOn(typeof(AbpImagingImageSharpModule))]
    public class TicketeraApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<TicketeraApplicationModule>();
        });
    }

    public override async Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        // -- Run init to refresh code for tickets
        var backgroundJobManager = context.ServiceProvider.GetService<IBackgroundJobManager>();
        if (backgroundJobManager != null) 
            await backgroundJobManager.EnqueueAsync(DateTime.Now);
                
        await base.OnPostApplicationInitializationAsync(context);
    }
}
