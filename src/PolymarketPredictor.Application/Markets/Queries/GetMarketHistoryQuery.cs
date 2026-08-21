using MediatR;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Application.Markets.Dtos;

namespace PolymarketPredictor.Application.Markets.Queries;

/// <summary>
/// Запрос полной истории прогнозов рынка, от старых к новым — для графика
/// </summary>
/// <param name="MarketId">Идентификатор рынка</param>
public sealed record GetMarketHistoryQuery(Guid MarketId) : IRequest<List<PredictionHistoryItemDto>>;

/// <summary>
/// Обработчик <see cref="GetMarketHistoryQuery"/>
/// </summary>
/// <param name="predictionRepository">Репозиторий прогнозов</param>
public sealed class GetMarketHistoryQueryHandler(IPredictionRepository predictionRepository) : IRequestHandler<GetMarketHistoryQuery, List<PredictionHistoryItemDto>>
{
    /// <inheritdoc />
    public async Task<List<PredictionHistoryItemDto>> Handle(GetMarketHistoryQuery request, CancellationToken ct)
    {
        var predictions = await predictionRepository.GetHistoryAsync(request.MarketId, ct);
        return [.. predictions.Select(p => new PredictionHistoryItemDto(p.PredictedProbability, p.ConfidenceScore, p.CreatedAt))];
    }
}