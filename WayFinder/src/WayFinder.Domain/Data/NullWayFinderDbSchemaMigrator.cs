using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace WayFinder.Data;

/* This is used if database provider does't define
 * IWayFinderDbSchemaMigrator implementation.
 */
public class NullWayFinderDbSchemaMigrator : IWayFinderDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
