using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.Application.Markets.Seed;

/// <summary>
/// Один вручную подобранный реальный рынок Polymarket для первоначального наполнения БД
/// Threshold/Direction/Asset уже извлечены вручную из текста вопроса, не полагаемся на regex
/// для MVP, чтобы не терять время на разбор неоднородных формулировок вопросов Polymarket
/// </summary>
/// <param name="ConditionId">conditionId рынка в Polymarket — использовать реальный, найденный на сайте перед демо</param>
/// <param name="Question">Точный текст вопроса, как на Polymarket</param>
/// <param name="AssetSymbol">Тикер актива (BTC, ETH и т.д.)</param>
/// <param name="CoinGeckoAssetId">Идентификатор актива в CoinGecko (например "bitcoin")</param>
/// <param name="ThresholdValue">Пороговое значение цены из вопроса</param>
/// <param name="Direction">Направление порогового условия</param>
/// <param name="ResolutionDate">Дата резолюции рынка</param>
public sealed record MarketSeedItem(string ConditionId, string Question, string AssetSymbol, string CoinGeckoAssetId, 
    decimal ThresholdValue, ThresholdDirection Direction, DateOnly ResolutionDate);