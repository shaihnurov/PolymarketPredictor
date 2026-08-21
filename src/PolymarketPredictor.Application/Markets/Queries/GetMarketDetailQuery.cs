using System.Text.Json;
using MediatR;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Application.Markets.Dtos;

namespace PolymarketPredictor.Application.Markets.Queries;

/// <summary>
/// Запрос детальной карточки одного рынка с последним прогнозом
/// </summary>
/// <param name="MarketId">Идентификатор рынка</param>
public sealed record GetMarketDetailQuery(Guid MarketId) : IRequest<MarketDetailDto?>;

/// <summary>
/// Обработчик <see cref="GetMarketDetailQuery"/>. Собирает DTO из рынка и его последнего прогноза,
/// десериализуя <see cref="Domain.Entities.Prediction.ArgumentsJson"/> обратно в <see cref="PredictionArguments"/>
/// </summary>
/// <param name="marketRepository">Репозиторий рынков</param>
/// <param name="predictionRepository">Репозиторий прогнозов</param>
public sealed class GetMarketDetailQueryHandler(ITrackedMarketRepository marketRepository, IPredictionRepository predictionRepository)
    : IRequestHandler<GetMarketDetailQuery, MarketDetailDto?>
{
    /// <inheritdoc />
    public async Task<MarketDetailDto?> Handle(GetMarketDetailQuery request, CancellationToken ct)
    {
        var market = await marketRepository.GetByIdAsync(request.MarketId, ct);

        if (market is null)
            return null;

        var latestPrediction = await predictionRepository.GetLatestAsync(market.Id, ct);

        PredictionDetailDto? predictionDetail = null;

        if (latestPrediction is not null)
        {
            var arguments = JsonSerializer.Deserialize<PredictionArguments>(latestPrediction.ArgumentsJson)
                ?? throw new InvalidOperationException($"Не удалось десериализовать ArgumentsJson прогноза {latestPrediction.Id}");

            predictionDetail = new PredictionDetailDto(latestPrediction.PredictedProbability, latestPrediction.ConfidenceScore, 
                latestPrediction.RiskNotes, arguments, latestPrediction.CreatedAt);
        }

        return new MarketDetailDto(
            market.Id,
            market.Question,
            market.AssetSymbol,
            market.ThresholdValue,
            market.Direction,
            market.ResolutionDate,
            market.Status,
            market.ActualOutcome,
            predictionDetail);
    }
}