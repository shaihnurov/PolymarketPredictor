using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.Domain.Entities;

/// <summary>
/// Один пороговый крипто-рынок Polymarket, который система отслеживает и по которому строит прогнозы.
/// Пример вопроса, который сюда попадает: "Will BTC be above $150,000 on Dec 31, 2026?"
/// </summary>
public class TrackedMarket
{
    /// <summary>
    /// Первичный ключ
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// conditionId рынка в Polymarket (Gamma API), уникальный ключ для сверки с источником
    /// </summary>
    public string PolymarketConditionId { get; set; } = default!;

    /// <summary>
    /// Исходный текст вопроса, как он звучит на Polymarket
    /// </summary>
    public string Question { get; set; } = default!;

    /// <summary>
    /// Тикер актива, извлечённый из вопроса: BTC, ETH и т.д
    /// </summary>
    public string AssetSymbol { get; set; } = default!;

    /// <summary>
    /// Идентификатор актива в CoinGecko (например "bitcoin" для BTC) — нужен для запросов цены/истории
    /// </summary>
    public string CoinGeckoAssetId { get; set; } = default!;

    /// <summary>
    /// Пороговое значение цены из вопроса
    /// </summary>
    public decimal ThresholdValue { get; set; }

    /// <summary>
    /// Направление порогового условия: выше или ниже <see cref="ThresholdValue"/>
    /// </summary>
    public ThresholdDirection Direction { get; set; }

    /// <summary>
    /// Дата резолюции рынка (UTC)
    /// </summary>
    public DateOnly ResolutionDate { get; set; }

    /// <summary>
    /// Текущий статус рынка: открыт или уже резолвлен
    /// </summary>
    public MarketStatus Status { get; set; } = MarketStatus.Open;

    /// <summary>
    /// Фактический исход после резолюции: true — условие сбылось, false — нет, null — рынок ещё открыт.
    /// Используется для расчёта Brier score
    /// </summary>
    public bool? ActualOutcome { get; set; }

    /// <summary>
    /// Момент, когда рынок впервые был добавлен в отслеживаемые (UTC)
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Все сырые снимки данных, собранные по этому рынку за всё время
    /// </summary>
    public List<RawSnapshot> RawSnapshots { get; set; } = [];

    /// <summary>
    /// Все нормализованные показатели, посчитанные по этому рынку за всё время
    /// </summary>
    public List<NormalizedIndicator> NormalizedIndicators { get; set; } = [];

    /// <summary>
    /// Полная история прогнозов по этому рынку (append-only лог)
    /// </summary>
    public List<Prediction> Predictions { get; set; } = [];
}