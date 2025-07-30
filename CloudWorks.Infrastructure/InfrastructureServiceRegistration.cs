using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Infrastructure.Common;
using CloudWorks.Infrastructure.Common.Interfaces;
using CloudWorks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace CloudWorks.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static void AddInfrastructureServices(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDataAccess(connectionString);
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CloudWorksDbContext>());
    }
    
    private static void AddDataAccess(this IServiceCollection services, string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");
        
        services.AddDbContext<CloudWorksDbContext>((provider, options) =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly("CloudWorks.Data.Migrations");
            }).EnableDetailedErrors();
        });
        
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