using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Reflection;

namespace Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration) =>
        services.AddMediator()
                .AddSerilog(configuration);

    private static IServiceCollection AddMediator(this IServiceCollection services) =>
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

    private static IServiceCollection AddSerilog(this IServiceCollection services, IConfiguration configuration) =>
        services.AddSerilog((_, cfg) => cfg.ReadFrom.Configuration(configuration));
}