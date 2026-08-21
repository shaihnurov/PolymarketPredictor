namespace PolymarketPredictor.Application.Markets.Services;

/// <summary>
/// Считает уверенность модели в конкретном прогнозе (не путать с самой вероятностью исхода) —
/// явная взвешенная сумма нормированных компонентов, все веса и формулы открыты и объяснимы
/// </summary>
public static class ConfidenceCalculator
{
    private const double VolumeWeight = 0.25;
    private const double LiquidityWeight = 0.25;
    private const double DataHistoryWeight = 0.20;
    private const double ProximityWeight = 0.30;

    private const decimal VolumeNormalizationCap = 50_000m;
    private const decimal LiquidityNormalizationCap = 50_000m;
    private const int DataHistoryNormalizationCapDays = 30;
    private const double ProximityNormalizationCapSigmas = 2.0;

    /// <summary>
    /// Считает итоговую уверенность прогноза, значение 0..1
    /// </summary>
    /// <param name="volume24h">Объём торгов рынка Polymarket за 24 часа, USD</param>
    /// <param name="liquidity">Ликвидность рынка Polymarket, USD</param>
    /// <param name="daysOfPriceHistoryUsed">Сколько дней ценовой истории фактически использовано при расчёте волатильности</param>
    /// <param name="distanceInSigmas">Расстояние от текущей цены до порога в сигмах (из <see cref="PredictionFormula"/>)</param>
    /// <returns>Уверенность, значение 0..1</returns>
    public static double Calculate(decimal volume24h, decimal liquidity, int daysOfPriceHistoryUsed, double distanceInSigmas)
    {
        var volumeScore = NormalizeToUnitRange(volume24h, VolumeNormalizationCap);
        var liquidityScore = NormalizeToUnitRange(liquidity, LiquidityNormalizationCap);
        var dataHistoryScore = NormalizeToUnitRange(daysOfPriceHistoryUsed, DataHistoryNormalizationCapDays);

        // Чем дальше цена от порога в сигмах, тем увереннее модель в направлении исхода
        // (не в конкретной цифре вероятности, а в том, что оценка не "на грани")
        var proximityScore = Math.Min(Math.Abs(distanceInSigmas) / ProximityNormalizationCapSigmas, 1.0);

        return VolumeWeight * volumeScore + LiquidityWeight * liquidityScore + DataHistoryWeight * dataHistoryScore + ProximityWeight * proximityScore;
    }

    /// <summary>
    /// Нормирует значение в [0, 1] делением на потолок, с отсечкой сверху
    /// </summary>
    private static double NormalizeToUnitRange(decimal value, decimal cap) =>
        Math.Min((double)(value / cap), 1.0);

    /// <summary>
    /// Нормирует целочисленное значение в [0, 1] делением на потолок, с отсечкой сверху
    /// </summary>
    private static double NormalizeToUnitRange(int value, int cap) =>
        Math.Min((double)value / cap, 1.0);
}