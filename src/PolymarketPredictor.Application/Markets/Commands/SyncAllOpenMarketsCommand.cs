using MediatR;
using Microsoft.Extensions.Logging;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Application.Markets.Dtos;

namespace PolymarketPredictor.Application.Markets.Commands;

/// <summary>
/// Команда одного полного цикла синка по всем открытым рынкам. Используется и фоновой службой
/// по расписанию (<c>MarketSyncBackgroundService</c>), и ручным HTTP-триггером для демо
/// единая точка оркестрации, без дублирования цикла в двух местах
/// </summary>
public sealed record SyncAllOpenMarketsCommand : IRequest<SyncAllMarketsResult>;

/// <summary>
/// Обработчик <see cref="SyncAllOpenMarketsCommand"/>. Ошибка синка одного рынка не должна
/// прерывать обработку остальных — каждый вызов <see cref="SyncMarketCommand"/> обёрнут в try/catch
/// </summary>
/// <param name="marketRepository">Репозиторий рынков</param>
/// <param name="sender">MediatR-отправитель для вызова <see cref="SyncMarketCommand"/> по каждому рынку</param>
/// <param name="logger">Логгер</param>
public sealed class SyncAllOpenMarketsCommandHandler(ITrackedMarketRepository marketRepository, ISender sender, 
    ILogger<SyncAllOpenMarketsCommandHandler> logger) : IRequestHandler<SyncAllOpenMarketsCommand, SyncAllMarketsResult>
{
    /// <inheritdoc />
    public async Task<SyncAllMarketsResult> Handle(SyncAllOpenMarketsCommand request, CancellationToken ct)
    {
        var openMarketIds = await marketRepository.GetOpenMarketIdsAsync(ct);

        var processed = 0;
        var failed = 0;

        foreach (var marketId in openMarketIds)
        {
            try
            {
                await sender.Send(new SyncMarketCommand(marketId), ct);
                processed++;
            }
            catch (Exception ex)
            {
                failed++;
                logger.LogError(ex, "SyncAllOpenMarkets: ошибка синка рынка {MarketId}.", marketId);
            }
        }

        return new SyncAllMarketsResult(processed, failed);
    }
}