namespace PolymarketPredictor.Domain.Entities;

/// <summary>
/// Нормализованные показатели одного цикла синка, посчитанные из <see cref="RawSnapshot"/>
/// обоих источников. Это единственный вход для формулы прогноза — формула никогда не читает
/// сырые данные напрямую, только эту таблицу
/// </summary>
public class NormalizedIndicator
{
    /// <summary>
    /// Первичный ключ
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Ссылка на рынок, к которому относятся эти показатели
    /// </summary>
    public Guid TrackedMarketId { get; set; }

    /// <summary>
    /// Навигационное свойство к рынку
    /// </summary>
    public TrackedMarket TrackedMarket { get; set; } = default!;

    /// <summary>
    /// Implied-вероятность из цены исхода "Yes" на Polymarket (outcomePrices[0]), 0..1
    /// </summary>
    public double PolymarketImpliedProbability { get; set; }

    /// <summary>
    /// Текущая спот-цена актива по CoinGecko, USD
    /// </summary>
    public decimal CurrentAssetPrice { get; set; }

    /// <summary>
    /// Дневная волатильность актива за последние 30 дней (стд. отклонение дневных log-доходностей)
    /// </summary>
    public double DailyVolatility30d { get; set; }

    /// <summary>
    /// Сколько полных дней осталось до даты резолюции на момент расчёта
    /// </summary>
    public int DaysToResolution { get; set; }

    /// <summary>
    /// Расстояние от текущей цены до порога в единицах волатильности
    /// Хранится отдельно, чтобы карточка прогноза могла показать её как готовый аргумент, не пересчитывая
    /// </summary>
    public double DistanceInSigmas { get; set; }

    /// <summary>
    /// Объём торгов на рынке Polymarket за 24 часа, USD
    /// </summary>
    public decimal Volume24h { get; set; }

    /// <summary>
    /// Ликвидность рынка Polymarket на момент снимка, USD
    /// </summary>
    public decimal Liquidity { get; set; }

    /// <summary>
    /// Сколько дней ценовой истории CoinGecko фактически использовано при расчёте волатильности
    /// </summary>
    public int DaysOfPriceHistoryUsed { get; set; }

    /// <summary>
    /// Момент, когда эти показатели были посчитаны (UTC)
    /// </summary>
    public DateTimeOffset ComputedAt { get; set; }

    /// <summary>
    /// Прогнозы, посчитанные на основе именно этого набора показателей
    /// </summary>
    public List<Prediction> Predictions { get; set; } = [];
}