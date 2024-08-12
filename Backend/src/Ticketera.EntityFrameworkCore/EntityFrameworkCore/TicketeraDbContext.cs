using Microsoft.EntityFrameworkCore;
using System.Linq;
using Ticketera.Events;
using Ticketera.Entities;
using Ticketera.TicketSectors;
using Ticketera.ProviderPayments;
using Ticketera.TicketCategories;
using Ticketera.EntityFrameworkCore.Config;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Ticketera.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class TicketeraDbContext : AbpDbContext<TicketeraDbContext>, IIdentityDbContext, ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventDate> EventDates { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketCategory> TicketCategories { get; set; }
    public DbSet<TicketSector> TicketSectors { get; set; }
    public DbSet<UserEvent> UserEvents { get; set; }
    public DbSet<ProviderPayment> ProviderPayments { get; set; }
    public DbSet<FileAttachment> FileAttachments { get; set; }
    public DbSet<EventProviderPayment> EventProviderPayments { get; set; }
    public DbSet<Purchase> Purchases { get; set; }

    #endregion

    public TicketeraDbContext(DbContextOptions<TicketeraDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        builder.ApplyConfiguration(new EventConfig());
        builder.ApplyConfiguration(new EventDateConfig());
        builder.ApplyConfiguration(new TicketCategoryConfig());
        builder.ApplyConfiguration(new TicketSectorConfig());
        builder.ApplyConfiguration(new TicketConfig());
        builder.ApplyConfiguration(new UserEventsConfig());
        builder.ApplyConfiguration(new ProviderPaymentConfig());
        builder.ApplyConfiguration(new FileAttachmentConfig());
        builder.ApplyConfiguration(new EventProviderPaymentConfig());
        builder.ApplyConfiguration(new PurchaseConfig());

        foreach (var relation in builder.Model.GetEntityTypes().SelectMany(x => x.GetForeignKeys()))
        {
            relation.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {        
        base.OnConfiguring(optionsBuilder);

        #if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
        #endif
    }
}
