using CloudWorks.Infrastructure;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CloudWorks.Application;

public static class ApplicationServiceRegistration
{
    public static void AddApplicationServices(
        this IServiceCollection services,
        string connectionString)
    {
        var applicationService = typeof(ApplicationServiceRegistration);
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(applicationService.Assembly);
        });
        services.AddValidatorsFromAssembly(applicationService.Assembly);
        services.AddInfrastructureServices(connectionString);
    }
}