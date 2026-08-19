using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PolymarketPredictor.Application.Common.Behaviors;

namespace PolymarketPredictor.Application.Extensions;

/// <summary>
/// Регистрация зависимостей слоя Application в DI-контейнере
/// </summary>
public static class ApplicationServiceExtensions
{
    /// <summary>
    /// Регистрирует сервисы
    /// </summary>
    /// <param name="services">Коллекция сервисов DI</param>
    /// <returns>Та же коллекция сервисов, для цепочки вызовов</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceExtensions).Assembly));

        services.AddValidatorsFromAssembly(typeof(ApplicationServiceExtensions).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}