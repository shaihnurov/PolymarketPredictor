using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.Domain.Entities;

/// <summary>
/// Сырой ответ внешнего источника (Polymarket Gamma API или CoinGecko) как есть, без интерпретации
/// Хранится, чтобы прогноз всегда можно было пересчитать/проверить по первоисточнику, а не только по уже посчитанным показателям
/// </summary>
public class RawSnapshot
{
    /// <summary>
    /// Первичный ключ
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Ссылка на рынок, к которому относится этот снимок
    /// </summary>
    public Guid TrackedMarketId { get; set; }

    /// <summary>
    /// Навигационное свойство к рынку
    /// </summary>
    public TrackedMarket TrackedMarket { get; set; } = default!;

    /// <summary>
    /// Из какого источника получен этот снимок — Polymarket или CoinGecko
    /// </summary>
    public SourceType SourceType { get; set; }

    /// <summary>
    /// Сырой JSON-ответ источника (хранится в PostgreSQL как jsonb).
    /// </summary>
    public string RawPayload { get; set; } = default!;

    /// <summary>
    /// Момент, когда снимок был фактически получен от источника (UTC)
    /// </summary>
    public DateTimeOffset FetchedAt { get; set; }
}