namespace PolymarketPredictor.Infrastructure.BackgroundJobs;

/// <summary>
/// Настройки периодичности фонового синка рынков, биндятся из секции "MarketSync" конфигурации
/// </summary>
public sealed class MarketSyncOptions
{
    /// <summary>
    /// Название секции конфигураци
    /// </summary>
    public const string SectionName = "MarketSync";

    /// <summary>
    /// Интервал между циклами синка, в минутах. По плану 15–30 минут
    /// </summary>
    public int IntervalMinutes { get; set; } = 20;
}