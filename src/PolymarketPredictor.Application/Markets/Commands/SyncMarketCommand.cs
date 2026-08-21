using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Application.Markets.Dtos;
using PolymarketPredictor.Application.Markets.Services;
using PolymarketPredictor.Domain.Entities;
using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.Application.Markets.Commands;

/// <summary>
/// Команда одного цикла синка одного рынка: забрать свежие данные из Polymarket и CoinGecko,
/// сохранить сырые снимки, посчитать нормализованные показатели и новый прогноз
/// </summary>
/// <param name="TrackedMarketId">Идентификатор рынка, который нужно синхронизировать</param>
public sealed record SyncMarketCommand(Guid TrackedMarketId) : IRequest;

/// <summary>
/// Обработчик <see cref="SyncMarketCommand"/>. Основной сценарий данных: Polymarket + CoinGecko →
/// RawSnapshot (оба источника) → NormalizedIndicator (детерминированно из raw) → Prediction
/// (детерминированно из NormalizedIndicator через <see cref="PredictionFormula"/>).
/// Каждый шаг — либо чтение внешнего API, либо чистая функция; никакой случайности.
/// </summary>
public sealed class SyncMarketCommandHandler(ITrackedMarketRepository marketRepository, IRawSnapshotRepository rawSnapshotRepository, INormalizedIndicatorRepository normalizedIndicatorRepository,
    IPredictionRepository predictionRepository, IPolymarketClient polymarketClient, ICoinGeckoClient coinGeckoClient, IUnitOfWork unitOfWork, 
    ILogger<SyncMarketCommandHandler> logger) : IRequestHandler<SyncMarketCommand>
{
    /// <summary>
    /// Глубина ценовой истории CoinGecko для расчёта волатильности
    /// </summary>
    private const int VolatilityHistoryDays = 30;

    /// <inheritdoc />
    public async Task Handle(SyncMarketCommand request, CancellationToken ct)
    {
        var market = await marketRepository.GetByIdAsync(request.TrackedMarketId, ct);

        if (market is null)
        {
            logger.LogWarning("SyncMarket: рынок {MarketId} не найден, пропускаю цикл", request.TrackedMarketId);
            return;
        }

        var polymarketData = await polymarketClient.GetMarketByConditionIdAsync(market.PolymarketConditionId, ct);

        if (polymarketData is null)
        {
            logger.LogWarning("SyncMarket: Polymarket не вернул данные по conditionId {ConditionId}, пропускаю цикл", market.PolymarketConditionId);
            return;
        }

        rawSnapshotRepository.Add(new RawSnapshot
        {
            Id = Guid.NewGuid(),
            TrackedMarketId = market.Id,
            SourceType = SourceType.Polymarket,
            RawPayload = JsonSerializer.Serialize(polymarketData),
            FetchedAt = DateTimeOffset.UtcNow
        });

        // Рынок уже резолвлен на стороне Polymarket — фиксируем фактический исход и на этом
        // цикл для этого рынка завершён: считать новый прогноз по закрытому рынку бессмысленно
        if (polymarketData.Closed)
        {
            market.Status = MarketStatus.Resolved;
            market.ActualOutcome = polymarketData.YesPrice >= 0.5;
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("SyncMarket: рынок {MarketId} резолвлен, ActualOutcome={ActualOutcome}", market.Id, market.ActualOutcome);
            return;
        }

        var currentPrice = await coinGeckoClient.GetCurrentPriceAsync(market.CoinGeckoAssetId, ct);
        var priceHistory = await coinGeckoClient.GetDailyPriceHistoryAsync(market.CoinGeckoAssetId, VolatilityHistoryDays, ct);

        rawSnapshotRepository.Add(new RawSnapshot
        {
            Id = Guid.NewGuid(),
            TrackedMarketId = market.Id,
            SourceType = SourceType.CoinGecko,
            RawPayload = JsonSerializer.Serialize(priceHistory),
            FetchedAt = DateTimeOffset.UtcNow
        });

        var dailyVolatility = VolatilityCalculator.CalculateDailyVolatility(priceHistory);
        var daysToResolution = Math.Max((market.ResolutionDate.ToDateTime(TimeOnly.MinValue) - DateTime.UtcNow.Date).Days, 0);
        var formulaResult = PredictionFormula.Calculate(currentPrice, market.ThresholdValue, market.Direction, dailyVolatility, daysToResolution);

        var indicator = new NormalizedIndicator
        {
            Id = Guid.NewGuid(),
            TrackedMarketId = market.Id,
            PolymarketImpliedProbability = polymarketData.YesPrice,
            CurrentAssetPrice = currentPrice,
            DailyVolatility30d = dailyVolatility,
            DaysToResolution = daysToResolution,
            DistanceInSigmas = formulaResult.DistanceInSigmas,
            Volume24h = polymarketData.Volume24hr,
            Liquidity = polymarketData.Liquidity,
            DaysOfPriceHistoryUsed = priceHistory.Count,
            ComputedAt = DateTimeOffset.UtcNow
        };
        normalizedIndicatorRepository.Add(indicator);

        var confidence = ConfidenceCalculator.Calculate(polymarketData.Volume24hr, polymarketData.Liquidity, priceHistory.Count, formulaResult.DistanceInSigmas);
        var riskNotes = RiskNoteGenerator.Generate(formulaResult.Probability, polymarketData.YesPrice, daysToResolution, dailyVolatility, polymarketData.Volume24hr);

        var arguments = new PredictionArguments(
            CurrentPrice: currentPrice,
            ThresholdValue: market.ThresholdValue,
            Direction: market.Direction.ToString(),
            DailyVolatility: dailyVolatility,
            DaysToResolution: daysToResolution,
            DistanceInSigmas: formulaResult.DistanceInSigmas,
            ModelProbability: formulaResult.Probability,
            MarketImpliedProbability: polymarketData.YesPrice,
            Volume24h: polymarketData.Volume24hr,
            Liquidity: polymarketData.Liquidity,
            DaysOfPriceHistoryUsed: priceHistory.Count);

        predictionRepository.Add(new Prediction
        {
            Id = Guid.NewGuid(),
            TrackedMarketId = market.Id,
            // Id индикатора уже сгенерирован в памяти (Guid, не identity из БД) — можно ссылаться
            // на него до SaveChanges, EF Core свяжет строки корректно в рамках одной транзакции
            NormalizedIndicatorId = indicator.Id,
            PredictedProbability = formulaResult.Probability,
            ConfidenceScore = confidence,
            RiskNotes = riskNotes,
            ArgumentsJson = JsonSerializer.Serialize(arguments),
            CreatedAt = DateTimeOffset.UtcNow
        });

        await unitOfWork.SaveChangesAsync(ct);
        logger.LogInformation("SyncMarket: рынок {MarketId} обновлён, вероятность={Probability:P1}, уверенность={Confidence:P1}.", market.Id, formulaResult.Probability, confidence);
    }
}