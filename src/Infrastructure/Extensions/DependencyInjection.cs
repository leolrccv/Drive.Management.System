using Application.Contracts;
using Infrastructure.Clients;
using Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Extensions;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) =>
       services.AddClients(configuration);

    private static IServiceCollection AddClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ClientSettings>(configuration.GetSection("ClientSettings"));

        services.AddHttpClient<IGeminiClient, GeminiClient>((serviceProvider, client) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<ClientSettings>>().Value;
            client.BaseAddress = new Uri(settings.GeminiApi.BaseAddress);
        });

        return services;
    }
}