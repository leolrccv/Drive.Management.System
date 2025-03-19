using Amazon;
using Amazon.S3;
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
       services.AddClients(configuration)
               .AddAws(configuration);

    private static IServiceCollection AddClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ClientSettings>(configuration.GetSection("ClientSettings"));

        services.AddHttpClient<IGeminiClient, GeminiClient>((serviceProvider, client) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<ClientSettings>>().Value;
            client.BaseAddress = new Uri(settings.GeminiApi.BaseAddress);
        });

        services.AddSingleton<IAwsClient, AwsClient>();

        return services;
    }

    private static IServiceCollection AddAws(this IServiceCollection services, IConfiguration configuration)
    {
        var awsSettings = configuration.GetSection("AwsSettings").Get<AwsSettings>();

        services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(
            awsSettings.Credentials.AccessKey,
            awsSettings.Credentials.SecretKey,
            RegionEndpoint.USEast1));

        return services;
    }
}