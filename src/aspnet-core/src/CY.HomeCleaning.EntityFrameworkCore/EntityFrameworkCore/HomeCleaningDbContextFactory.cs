using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CY.HomeCleaning.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class HomeCleaningDbContextFactory : IDesignTimeDbContextFactory<HomeCleaningDbContext>
{
    public HomeCleaningDbContext CreateDbContext(string[] args)
    {
        HomeCleaningEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<HomeCleaningDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));

        return new HomeCleaningDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../CY.HomeCleaning.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
