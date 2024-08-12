using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ticketera.EntityFrameworkCore;
using Ticketera.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.Auditing;
using Volo.Abp.Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Volo.Abp.OpenIddict;
using System.Security.Cryptography.X509Certificates;

namespace Ticketera;

[DependsOn(
    typeof(TicketeraHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(TicketeraApplicationModule),
    typeof(TicketeraEntityFrameworkCoreModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
)]
public class TicketeraHttpApiHostModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();
        
        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("Ticketera");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });

        if (hostingEnvironment.IsProduction())
        {

            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });

            var certificate = GetCertificateFromResources();
            PreConfigure<OpenIddictServerBuilder>(builder =>
            {
                builder.AddEncryptionCertificate(certificate);
                builder.AddSigningCertificate(certificate);
                builder.SetIssuer(new Uri(configuration["AuthServer:Authority"]!));
            });
        }
    }

    private X509Certificate2 GetCertificateFromResources()
    {
        // Obtener la contraseña del certificado desde la variable de entorno
        string certPassword = Environment.GetEnvironmentVariable("CERT_PASSWORD");

        if (string.IsNullOrEmpty(certPassword))
        {
            throw new InvalidOperationException("La contraseña del certificado no está configurada.");
        }

        // Obtener el recurso binario del certificado
        byte[] certBytes = Resource.TixgoCert;

        // Crear el certificado X509 a partir del recurso binario
        X509Certificate2 certificate = new X509Certificate2(certBytes, certPassword, X509KeyStorageFlags.MachineKeySet);

        return certificate;
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        Configure<AbpAuditingOptions>(options =>
        {
            options.IsEnabled = false; 
        });

        ConfigureAuthentication(context);
        ConfigureBundles();
        ConfigureUrls(configuration);
        ConfigureConventionalControllers();
        ConfigureVirtualFileSystem(context);
        ConfigureCors(context, configuration);
        ConfigureSwaggerServices(context, configuration);
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
    }

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonXLiteThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<string>());

            options.Applications["Angular"].RootUrl = configuration["App:ClientUrl"];
            options.Applications["Angular"].Urls[AccountUrlNames.PasswordReset] = "account/reset-password";
        });
    }

    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsEnvironment("Local"))
        {
            //Configure<AbpVirtualFileSystemOptions>(options =>
            //{
            //    options.FileSets.ReplaceEmbeddedByPhysical<TicketeraDomainSharedModule>(
            //        Path.Combine(hostingEnvironment.ContentRootPath,
            //            $"..{Path.DirectorySeparatorChar}Ticketera.Domain.Shared"));
            //    options.FileSets.ReplaceEmbeddedByPhysical<TicketeraDomainModule>(
            //        Path.Combine(hostingEnvironment.ContentRootPath,
            //            $"..{Path.DirectorySeparatorChar}Ticketera.Domain"));
            //    options.FileSets.ReplaceEmbeddedByPhysical<TicketeraApplicationContractsModule>(
            //        Path.Combine(hostingEnvironment.ContentRootPath,
            //            $"..{Path.DirectorySeparatorChar}Ticketera.Application.Contracts"));
            //    options.FileSets.ReplaceEmbeddedByPhysical<TicketeraApplicationModule>(
            //        Path.Combine(hostingEnvironment.ContentRootPath,
            //            $"..{Path.DirectorySeparatorChar}Ticketera.Application"));
            //});
        }
    }

    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(TicketeraApplicationModule).Assembly);
        });
    }

    private static void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAbpSwaggerGenWithOAuth(
            configuration["AuthServer:Authority"]!,
            new Dictionary<string, string>
            {
                    {"Ticketera", "Ticketera API"}
            },
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Ticketera API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            });
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(configuration["App:CorsOrigins"]?
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(o => o.RemovePostFix("/"))
                        .ToArray() ?? Array.Empty<string>())
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsEnvironment("Local"))
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsEnvironment("Local"))
        {
            app.UseErrorPage();
        }

        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        // -- Add Hangfire Dashboard
        app.UseAbpHangfireDashboard("/hangfire", options =>
        {
            options.AsyncAuthorization = new[] { new AbpHangfireAuthorizationFilter() };
        });

        app.UseUnitOfWork();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseAbpSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticketera API");

            var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
            c.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            c.OAuthScopes("Ticketera");
        });

        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();

        if (env.IsDevelopment() || env.IsProduction())
        {
            app.UseHttpsRedirection(); // Redirecciona a HTTPS en los entornos de desarrollo y producción
        }
    }
}
