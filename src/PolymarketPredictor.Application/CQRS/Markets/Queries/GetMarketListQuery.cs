using MediatR;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Application.CQRS.Markets.Dtos;

namespace PolymarketPredictor.Application.CQRS.Markets.Queries;

/// <summary>
/// Запрос списка всех отслеживаемых рынков с их последним прогнозом, для экрана списка
/// </summary>
public sealed record GetMarketListQuery : IRequest<List<MarketListItemDto>>;

/// <summary>
/// Обработчик <see cref="GetMarketListQuery"/>. Собирает DTO из двух репозиториев:
/// список рынков и словарь их последних прогнозов одним пакетным запросом
/// </summary>
/// <param name="marketRepository">Репозиторий рынков</param>
/// <param name="predictionRepository">Репозиторий прогнозов</param>
public sealed class GetMarketListQueryHandler(ITrackedMarketRepository marketRepository, IPredictionRepository predictionRepository)
    : IRequestHandler<GetMarketListQuery, List<MarketListItemDto>>
{
    /// <inheritdoc />
    public async Task<List<MarketListItemDto>> Handle(GetMarketListQuery request, CancellationToken ct)
    {
        var markets = await marketRepository.GetAllAsync(ct);
        var latestPredictions = await predictionRepository.GetLatestByMarketIdsAsync([.. markets.Select(m => m.Id)], ct);

        return [.. markets.Select(m =>
        {
            latestPredictions.TryGetValue(m.Id, out var latest);

            return new MarketListItemDto(m.Id, m.Question, m.AssetSymbol, m.ThresholdValue, m.Direction, m.ResolutionDate, m.Status,
                latest?.PredictedProbability, latest?.ConfidenceScore, latest?.CreatedAt);
        })];
    }
}