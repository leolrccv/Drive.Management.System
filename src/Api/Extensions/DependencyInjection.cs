using Api.Providers;
using Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services) =>
        services.AddBase()
                .AddFormOptions()
                .AddErrorProvider()
                .AddFluentValidation();

    private static IServiceCollection AddBase(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(GetJsonOptions());

        return services.AddEndpointsApiExplorer()
                       .AddSwaggerGen()
                       .AddCors(GetCorsOptions());
    }

    private static IServiceCollection AddFormOptions(this IServiceCollection services) =>
        services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = 10 * 1024 * 1024);

    private static IServiceCollection AddErrorProvider(this IServiceCollection services) =>
        services.AddScoped<IErrorProvider, ErrorProvider>();

    private static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationHandler<,>));
        return services.AddValidatorsFromAssemblies(GetAssembliesWithValidator());
    }

    private static IEnumerable<Assembly> GetAssembliesWithValidator() =>
        AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => assembly.GetTypes().Any(
                type => typeof(IValidator).IsAssignableFrom(type) &&
                type.Assembly != typeof(IValidator).Assembly));

    private static Action<JsonOptions> GetJsonOptions() =>
        options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    private static Action<CorsOptions> GetCorsOptions() =>
        options => options.AddPolicy("AllowAll", policy =>
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}