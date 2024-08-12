using System.Threading.Tasks;

namespace Ticketera.Data;

public interface ITicketeraDbSchemaMigrator
{
    Task MigrateAsync();
}
