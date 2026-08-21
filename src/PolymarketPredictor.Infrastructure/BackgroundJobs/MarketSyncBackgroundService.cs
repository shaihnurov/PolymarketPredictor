using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PolymarketPredictor.Application.Markets.Commands;

namespace PolymarketPredictor.Infrastructure.BackgroundJobs;

/// <summary>
/// Фоновая служба, которая каждые <see cref="MarketSyncOptions.IntervalMinutes"/> минут забирает
/// список всех открытых рынков и запускает <see cref="SyncMarketCommand"/> для каждого.
/// Только оркестрация по расписанию — вся бизнес-логика синка одного рынка находится в
/// SyncMarketCommandHandler (Application), эта служба его не дублирует
/// </summary>
/// <param name="scopeFactory">Фабрика DI-скоупов репозитории и MediatR скоупные, а служба — singleton</param>
/// <param name="options">Настройки периодичности синка</param>
/// <param name="logger">Логгер</param>
public sealed class MarketSyncBackgroundService(IServiceScopeFactory scopeFactory, IOptions<MarketSyncOptions> options, ILogger<MarketSyncBackgroundService> logger) : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromMinutes(options.Value.IntervalMinutes);
        using var timer = new PeriodicTimer(interval);

        logger.LogInformation("MarketSyncBackgroundService запущен, интервал синка: {Interval}", interval);

        // Первый цикл — сразу при старте, не дожидаясь первого тика таймера, чтобы данные
        // появились в БД без ожидания полного интервала после запуска приложения
        await RunSyncCycleAsync(stoppingToken);

        while (await timer.WaitForNextTickAsync(stoppingToken))
            await RunSyncCycleAsync(stoppingToken);
    }

    /// <summary>
    /// Запускает один цикл <see cref="SyncAllOpenMarketsCommand"/> в новом DI-скоупе
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    private async Task RunSyncCycleAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        try
        {
            var result = await sender.Send(new SyncAllOpenMarketsCommand(), ct);
            logger.LogInformation("MarketSyncBackgroundService: цикл завершён, обработано {Processed}, ошибок {Failed}.", result.MarketsProcessed, result.MarketsFailed);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "MarketSyncBackgroundService: цикл синка завершился с ошибкой.");
        }
    }
}