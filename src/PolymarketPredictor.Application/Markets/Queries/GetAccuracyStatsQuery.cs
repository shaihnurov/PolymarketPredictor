using MediatR;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Application.Markets.Dtos;

namespace PolymarketPredictor.Application.Markets.Queries;

/// <summary>
/// Запрос метрики калибровки модели (Brier score) по всем резолвленным рынкам
/// </summary>
public sealed record GetAccuracyStatsQuery : IRequest<AccuracyStatsDto>;

/// <summary>
/// Обработчик <see cref="GetAccuracyStatsQuery"/>. Brier score считается по последнему прогнозу
/// каждого резолвленного рынка (не по всей истории) — это финальная оценка модели перед резолюцией,
/// именно её и имеет смысл сверять с фактическим исходом
/// </summary>
/// <param name="marketRepository">Репозиторий рынков</param>
/// <param name="predictionRepository">Репозиторий прогнозов</param>
public sealed class GetAccuracyStatsQueryHandler(ITrackedMarketRepository marketRepository, IPredictionRepository predictionRepository) 
    : IRequestHandler<GetAccuracyStatsQuery, AccuracyStatsDto>
{
    /// <inheritdoc />
    public async Task<AccuracyStatsDto> Handle(GetAccuracyStatsQuery request, CancellationToken ct)
    {
        var resolvedMarkets = await marketRepository.GetResolvedMarketsAsync(ct);

        if (resolvedMarkets.Count == 0)
            return new AccuracyStatsDto(ResolvedMarketsCount: 0, MarketsIncludedInScore: 0, BrierScore: null);

        var latestPredictions = await predictionRepository.GetLatestByMarketIdsAsync([.. resolvedMarkets.Select(m => m.Id)], ct);
        var squaredErrors = new List<double>();

        foreach (var market in resolvedMarkets)
        {
            if (!latestPredictions.TryGetValue(market.Id, out var prediction))
                continue;

            var actualOutcomeAsDouble = market.ActualOutcome!.Value ? 1.0 : 0.0;
            var squaredError = Math.Pow(prediction.PredictedProbability - actualOutcomeAsDouble, 2);
            squaredErrors.Add(squaredError);
        }

        var brierScore = squaredErrors.Count > 0 ? squaredErrors.Average() : (double?)null;
        return new AccuracyStatsDto(resolvedMarkets.Count, squaredErrors.Count, brierScore);
    }
}