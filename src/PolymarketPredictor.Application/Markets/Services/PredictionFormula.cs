using MathNet.Numerics.Distributions;
using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.Application.Markets.Services;

/// <summary>
/// Ядро прогноза: геометрическое случайное блуждание без сноса (аналог величины d2 из
/// Black Scholes для бинарного исхода), на исторической волатильности вместо implied
/// Чистая функция без побочных эффектов — детерминированно считает вероятность
/// из уже сохранённых показателей, ничего не читает и не пишет в БД
/// </summary>
public static class PredictionFormula
{
    /// <summary>
    /// Результат расчёта формулы прогноза
    /// </summary>
    /// <param name="Probability">Вероятность того, что пороговое условие сбудется к дате резолюции, 0..1</param>
    /// <param name="DistanceInSigmas">Расстояние от текущей цены до порога в единицах волатильности (величина d)</param>
    public readonly record struct PredictionResult(double Probability, double DistanceInSigmas);

    /// <summary>
    /// Считает вероятность порогового условия по модели геометрического случайного блуждания
    /// </summary>
    /// <param name="currentPrice">Текущая цена актива</param>
    /// <param name="thresholdValue">Пороговое значение из условия рынка</param>
    /// <param name="direction">Направление условия — выше или ниже порога</param>
    /// <param name="dailyVolatility">Дневная волатильность актива (см. <see cref="VolatilityCalculator"/>)</param>
    /// <param name="daysToResolution">Сколько дней осталось до даты резолюции</param>
    /// <returns>Вероятность и расстояние в сигмах</returns>
    /// <exception cref="ArgumentOutOfRangeException">Если цена, порог или волатильность не положительны</exception>
    public static PredictionResult Calculate(decimal currentPrice, decimal thresholdValue, ThresholdDirection direction, 
        double dailyVolatility, int daysToResolution)
    {
        if (currentPrice <= 0)
            throw new ArgumentOutOfRangeException(nameof(currentPrice), "Текущая цена должна быть положительной");

        if (thresholdValue <= 0)
            throw new ArgumentOutOfRangeException(nameof(thresholdValue), "Порог должен быть положительным");

        if (dailyVolatility < 0)
            throw new ArgumentOutOfRangeException(nameof(dailyVolatility), "Волатильность не может быть отрицательной");

        if (daysToResolution <= 0)
        {
            var conditionAlreadyTrue = direction == ThresholdDirection.Above ? currentPrice >= thresholdValue : currentPrice <= thresholdValue;
            return new PredictionResult(conditionAlreadyTrue ? 1.0 : 0.0, DistanceInSigmas: 0.0);
        }

        if (dailyVolatility == 0)
        {
            var thresholdAlreadyMet = direction == ThresholdDirection.Above ? currentPrice >= thresholdValue : currentPrice <= thresholdValue;
            return new PredictionResult(thresholdAlreadyMet ? 1.0 : 0.0, DistanceInSigmas: double.PositiveInfinity);
        }

        var d = Math.Log((double)(thresholdValue / currentPrice)) / (dailyVolatility * Math.Sqrt(daysToResolution));

        // Φ(d) — функция стандартного нормального распределения в точке d
        var probability = direction == ThresholdDirection.Above ? 1.0 - Normal.CDF(0, 1, d) : Normal.CDF(0, 1, d);

        return new PredictionResult(probability, DistanceInSigmas: d);
    }
}