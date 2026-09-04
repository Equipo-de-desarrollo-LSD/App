using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace WayFinder.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class WayFinderDbContextFactory : IDesignTimeDbContextFactory<WayFinderDbContext>
{
    public WayFinderDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        WayFinderEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<WayFinderDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new WayFinderDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../WayFinder.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
