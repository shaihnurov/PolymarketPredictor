using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.Application.CQRS.Markets.Dtos;

/// <summary>
/// Проекция одного рынка для экрана списка — только то, что нужно карточке в гриде,
/// без дочерних коллекций сырых снимков и полной истории прогнозов
/// </summary>
/// <param name="Id">Идентификатор рынка</param>
/// <param name="Question">Текст вопроса рынка</param>
/// <param name="AssetSymbol">Тикер актива (BTC, ETH и т.д)</param>
/// <param name="ThresholdValue">Пороговое значение цены</param>
/// <param name="Direction">Направление порогового условия</param>
/// <param name="ResolutionDate">Дата резолюции рынка</param>
/// <param name="Status">Текущий статус рынка</param>
/// <param name="LatestPredictedProbability">Вероятность из последнего прогноза, если он есть</param>
/// <param name="LatestConfidenceScore">Уверенность последнего прогноза, если он есть</param>
/// <param name="LatestPredictionAt">Момент последнего прогноза, если он есть</param>
public sealed record MarketListItemDto(Guid Id, string Question, string AssetSymbol, decimal ThresholdValue, ThresholdDirection Direction,
    DateOnly ResolutionDate, MarketStatus Status, double? LatestPredictedProbability, double? LatestConfidenceScore, DateTimeOffset? LatestPredictionAt);