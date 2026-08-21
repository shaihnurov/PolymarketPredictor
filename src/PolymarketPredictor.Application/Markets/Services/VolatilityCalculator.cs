using PolymarketPredictor.Application.Common.Models.CoinGecko;

namespace PolymarketPredictor.Application.Markets.Services;

/// <summary>
/// Считает историческую волатильность актива по дневной ценовой истории CoinGecko
/// </summary>
public static class VolatilityCalculator
{
    /// <summary>
    /// Считает дневную волатильность (стандартное отклонение log-доходностей) по истории цен
    /// </summary>
    /// <param name="priceHistory">Дневная история цен, отсортированная по возрастанию даты. Нужно минимум 2 точки</param>
    /// <returns>Дневная волатильность как доля (например 0.03 = 3% в день)</returns>
    /// <exception cref="ArgumentException">Если точек меньше двух или встретилась цена <= 0</exception>
    public static double CalculateDailyVolatility(IReadOnlyList<DailyPricePoint> priceHistory)
    {
        if (priceHistory.Count < 2)
            throw new ArgumentException("Для расчёта волатильности нужно минимум 2 точки истории цен", nameof(priceHistory));

        var logReturns = new List<double>(priceHistory.Count - 1);

        for (var i = 1; i < priceHistory.Count; i++)
        {
            var previous = priceHistory[i - 1].ClosePrice;
            var current = priceHistory[i].ClosePrice;

            if (previous <= 0 || current <= 0)
                throw new ArgumentException("Цена в истории не может быть нулевой или отрицательной", nameof(priceHistory));

            logReturns.Add(Math.Log((double)(current / previous)));
        }

        var mean = logReturns.Average();
        var sumSquaredDeviations = logReturns.Sum(r => Math.Pow(r - mean, 2));
        var variance = sumSquaredDeviations / (logReturns.Count - 1);

        return Math.Sqrt(variance);
    }
}