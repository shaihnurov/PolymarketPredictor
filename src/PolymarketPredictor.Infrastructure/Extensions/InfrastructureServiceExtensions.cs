using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Infrastructure.BackgroundJobs;
using PolymarketPredictor.Infrastructure.ExternalClients.CoinGecko;
using PolymarketPredictor.Infrastructure.ExternalClients.Polymarket;

namespace PolymarketPredictor.Infrastructure.Extensions;

/// <summary>Р
/// егистрация зависимостей слоя Infrastructure в DI-контейнере
/// </summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Регистрирует сервисы
    /// </summary>
    /// <param name="services">Коллекция сервисов DI</param>
    /// <returns>Та же коллекция сервисов, для цепочки вызовов</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CoinGeckoOptions>(configuration.GetSection(CoinGeckoOptions.SectionName));

        services.AddHttpClient<IPolymarketClient, PolymarketClient>(client =>
        {
            client.BaseAddress = new Uri("https://gamma-api.polymarket.com/");
            client.Timeout = TimeSpan.FromSeconds(15);
        }).AddStandardResilienceHandler();

        services.AddHttpClient<ICoinGeckoClient, CoinGeckoClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<CoinGeckoOptions>>().Value;

            client.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");
            client.Timeout = TimeSpan.FromSeconds(15);

            if (!string.IsNullOrWhiteSpace(options.ApiKey))
                client.DefaultRequestHeaders.Add("x-cg-demo-api-key", options.ApiKey);
        }).AddStandardResilienceHandler();

        services.Configure<MarketSyncOptions>(configuration.GetSection(MarketSyncOptions.SectionName));
        services.AddHostedService<MarketSyncBackgroundService>();

        return services;
    }
}