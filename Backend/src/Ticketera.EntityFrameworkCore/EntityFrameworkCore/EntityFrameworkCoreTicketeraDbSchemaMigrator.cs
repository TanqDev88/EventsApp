using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ticketera.Data;
using Volo.Abp.DependencyInjection;

namespace Ticketera.EntityFrameworkCore;

public class EntityFrameworkCoreTicketeraDbSchemaMigrator
    : ITicketeraDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreTicketeraDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the TicketeraDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<TicketeraDbContext>()
            .Database
            .MigrateAsync();
    }
}
