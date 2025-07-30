using CloudWorks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CloudWorks.Data.Migrations;
public class DbContextFactory() : IDesignTimeDbContextFactory<CloudWorksDbContext>
{
    private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    private const string ASSEMBLY_NAME = "CloudWorks.Data.Migrations";

    private const string CONNECTION_NAME = "DefaultDb";

    public CloudWorksDbContext CreateDbContext(string[] args)
    {
        string? connectionString = GetConnectionString(Configuration, CONNECTION_NAME);
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"{nameof(connectionString)} cannot be null or empty");
        }

        var builder = new DbContextOptionsBuilder<CloudWorksDbContext>();

        builder.UseNpgsql(
            connectionString,
            b =>
            {
                b.MigrationsAssembly(ASSEMBLY_NAME);
                b.CommandTimeout((int)TimeSpan.FromMinutes(100).TotalSeconds);
            }
        );

        return new CloudWorksDbContext(builder.Options);
    }

    private static string GetConnectionString(IConfiguration configuration, string key)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(configuration.GetConnectionString(key))
        {
            Username = configuration[$"{key}:User"],
            Password = configuration[$"{key}:Password"]
        };

        return connectionStringBuilder.ToString();
    }
}
