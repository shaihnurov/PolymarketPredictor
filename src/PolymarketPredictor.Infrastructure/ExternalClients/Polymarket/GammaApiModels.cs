namespace PolymarketPredictor.Infrastructure.ExternalClients.Polymarket;

/// <summary>
/// Модель рынка Gamma API как есть в JSON-ответе
/// </summary>
internal sealed class GammaMarketApiModel
{
    /// <summary>
    /// Уникальный идентификатор рынка (condition ID) в Polymarket
    /// </summary>
    public string ConditionId { get; set; } = default!;

    /// <summary>
    /// Текст вопроса рынка
    /// </summary>
    public string Question { get; set; } = default!;

    /// <summary>
    /// Рынок торгуется в данный момент
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Рынок уже закрыт (резолвлен)
    /// </summary>
    public bool Closed { get; set; }

    /// <summary>
    /// JSON-строка вида "[\"Yes\", \"No\"]" — Gamma отдаёт массивы как строки, не как нативный JSON-массив
    /// </summary>
    public string? Outcomes { get; set; }

    /// <summary>
    /// JSON-строка вида "[\"0.20\", \"0.80\"]". Индекс 0 = цена исхода "Yes" = implied-вероятность
    /// </summary>
    public string? OutcomePrices { get; set; }

    /// <summary>
    /// Объём торгов за последние 24 часа, USD
    /// </summary>
    public decimal? Volume24hr { get; set; }

    /// <summary>
    /// Текущая ликвидность рынка, USD
    /// </summary>
    public decimal? Liquidity { get; set; }
}