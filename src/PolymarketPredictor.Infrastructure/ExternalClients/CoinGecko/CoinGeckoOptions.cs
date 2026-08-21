namespace PolymarketPredictor.Infrastructure.ExternalClients.CoinGecko;

/// <summary>
/// Настройки клиента CoinGecko, биндятся из секции "CoinGecko" конфигурации
/// </summary>
public sealed class CoinGeckoOptions
{
    /// <summary>
    /// Название секции конфигурации
    /// </summary>
    public const string SectionName = "CoinGecko";

    /// <summary>
    /// Demo API-ключ CoinGecko
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
}