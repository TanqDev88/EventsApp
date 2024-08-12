using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Ticketera.Data;

/* This is used if database provider does't define
 * ITicketeraDbSchemaMigrator implementation.
 */
public class NullTicketeraDbSchemaMigrator : ITicketeraDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
