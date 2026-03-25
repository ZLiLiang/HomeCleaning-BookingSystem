using System.Threading.Tasks;

namespace CY.HomeCleaning.Data;

public interface IHomeCleaningDbSchemaMigrator
{
    Task MigrateAsync();
}
