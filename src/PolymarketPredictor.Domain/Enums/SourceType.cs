namespace PolymarketPredictor.Domain.Enums;

/// <summary>
/// Реальный внешний источник данных, из которого получен <see cref="Entities.RawSnapshot"/>
/// </summary>
public enum SourceType
{
    /// <summary>
    /// Данные получены из Polymarket Gamma API
    /// </summary>
    Polymarket,

    /// <summary>
    /// Данные получены из CoinGecko API
    /// </summary>
    CoinGecko
}