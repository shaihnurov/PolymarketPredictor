using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PolymarketPredictor.Application.Common.Interfaces;

namespace PolymarketPredictor.Persistence;

/// <summary>
/// Регистрация зависимостей слоя Persistence в DI-контейнере
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Регистрирует сервисы
    /// </summary>
    /// <param name="services">Коллекция сервисов DI</param>
    /// <param name="configuration">Конфигурация приложения, откуда берётся строка подключения "Postgres"</param>
    /// <returns>Та же коллекция сервисов, для цепочки вызовов</returns>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Connection string 'Postgres' не найдена в конфигурации");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString, npgsql 
            => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        return services;
    }
}