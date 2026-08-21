using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PolymarketPredictor.Application.Common.Interfaces;
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
    /// Выполняет один полный цикл синка по всем открытым рынкам
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    private async Task RunSyncCycleAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var marketRepository = scope.ServiceProvider.GetRequiredService<ITrackedMarketRepository>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        List<Guid> openMarketIds;
        try
        {
            openMarketIds = await marketRepository.GetOpenMarketIdsAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "MarketSyncBackgroundService: не удалось получить список открытых рынков");
            return;
        }

        logger.LogInformation("MarketSyncBackgroundService: начинаю цикл синка, рынков: {Count}", openMarketIds.Count);

        foreach (var marketId in openMarketIds)
        {
            try
            {
                await mediator.Send(new SyncMarketCommand(marketId), ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "MarketSyncBackgroundService: ошибка синка рынка {MarketId}.", marketId);
            }
        }
    }
}