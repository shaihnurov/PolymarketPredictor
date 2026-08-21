using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.Application.Markets.Dtos;

/// <summary>
/// Полная карточка одного рынка с деталями последнего прогноза, для экрана деталей
/// </summary>
/// <param name="Id">Идентификатор рынка в нашей БД</param>
/// <param name="Question">Текст вопроса рынка</param>
/// <param name="AssetSymbol">Тикер актива</param>
/// <param name="ThresholdValue">Пороговое значение цены</param>
/// <param name="Direction">Направление порогового условия</param>
/// <param name="ResolutionDate">Дата резолюции рынка</param>
/// <param name="Status">Текущий статус рынка</param>
/// <param name="ActualOutcome">Фактический исход, если рынок уже резолвлен</param>
/// <param name="LatestPrediction">Последний прогноз по рынку, либо null, если прогнозов ещё не было</param>
public sealed record MarketDetailDto(
    Guid Id,
    string Question,
    string AssetSymbol,
    decimal ThresholdValue,
    ThresholdDirection Direction,
    DateOnly ResolutionDate,
    MarketStatus Status,
    bool? ActualOutcome,
    PredictionDetailDto? LatestPrediction);