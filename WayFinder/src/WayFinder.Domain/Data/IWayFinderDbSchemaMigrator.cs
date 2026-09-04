using System.Threading.Tasks;

namespace WayFinder.Data;

public interface IWayFinderDbSchemaMigrator
{
    Task MigrateAsync();
}
