namespace PolymarketPredictor.Application.Markets.Services;

/// <summary>
/// Генерирует человекочитаемые причины, почему прогноз может не сбыться,
/// без ML, каждое правило проверяется независимо и добавляет свою заметку при срабатывании
/// </summary>
public static class RiskNoteGenerator
{
    private const double MarketDivergenceThreshold = 0.15;
    private const int ShortHorizonThresholdDays = 3;
    private const double HighVolatilityThreshold = 0.05;
    private const decimal LowVolumeThreshold = 5_000m;

    /// <summary>
    /// Собирает список риск-заметок по условиям, которые сработали для данного прогноза
    /// </summary>
    /// <param name="modelProbability">Вероятность, посчитанная моделью (см. <see cref="PredictionFormula"/>)</param>
    /// <param name="marketImpliedProbability">Implied-вероятность из цены Polymarket</param>
    /// <param name="daysToResolution">Сколько дней осталось до даты резолюции</param>
    /// <param name="dailyVolatility">Дневная волатильность актива</param>
    /// <param name="volume24h">Объём торгов рынка Polymarket за 24 часа, USD</param>
    /// <returns>Список риск-заметок; пустой список, если ни одно правило не сработало</returns>
    public static List<string> Generate(double modelProbability, double marketImpliedProbability, int daysToResolution, double dailyVolatility, decimal volume24h)
    {
        var notes = new List<string>();

        if (Math.Abs(modelProbability - marketImpliedProbability) > MarketDivergenceThreshold)
            notes.Add("Большое расхождение с рыночной ценой Polymarket — модель и рынок расходятся во мнении.");

        if (daysToResolution < ShortHorizonThresholdDays)
            notes.Add("Короткий горизонт до резолюции — оценка менее надёжна на таком коротком интервале.");

        if (dailyVolatility > HighVolatilityThreshold)
            notes.Add("Повышенная волатильность актива — фактический исход может сильно отличаться от оценки.");

        if (volume24h < LowVolumeThreshold)
            notes.Add("Низкий объём торгов на рынке Polymarket — implied-вероятность может быть шумной.");

        return notes;
    }
}