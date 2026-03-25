using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CY.HomeCleaning.Data;
using Volo.Abp.DependencyInjection;

namespace CY.HomeCleaning.EntityFrameworkCore;

public class EntityFrameworkCoreHomeCleaningDbSchemaMigrator
    : IHomeCleaningDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreHomeCleaningDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the HomeCleaningDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<HomeCleaningDbContext>()
            .Database
            .MigrateAsync();
    }
}
