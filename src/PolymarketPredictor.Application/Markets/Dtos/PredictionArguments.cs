namespace PolymarketPredictor.Application.Markets.Dtos;

/// <summary>
/// Полная разбивка входных данных и промежуточных величин, из которых посчитан конкретный
/// прогноз. Сериализуется в <see cref="Domain.Entities.Prediction.ArgumentsJson"/> это то,
/// что карточка прогноза показывает пользователю как "почему именно такая цифра"
/// </summary>
/// <param name="CurrentPrice">Текущая цена актива на момент расчёта</param>
/// <param name="ThresholdValue">Пороговое значение из условия рынка</param>
/// <param name="Direction">Направление условия — строкой ("Above"/"Below"), для читаемости в JSON</param>
/// <param name="DailyVolatility">Дневная волатильность актива, использованная в расчёте</param>
/// <param name="DaysToResolution">Сколько дней оставалось до резолюции на момент расчёта</param>
/// <param name="DistanceInSigmas">Расстояние от текущей цены до порога в единицах волатильности</param>
/// <param name="ModelProbability">Вероятность, посчитанная моделью</param>
/// <param name="MarketImpliedProbability">Implied-вероятность из цены Polymarket на тот же момент</param>
/// <param name="Volume24h">Объём торгов рынка Polymarket за 24 часа, использованный в расчёте уверенности</param>
/// <param name="Liquidity">Ликвидность рынка Polymarket, использованная в расчёте уверенности</param>
/// <param name="DaysOfPriceHistoryUsed">Сколько дней ценовой истории CoinGecko фактически использовано</param>
public sealed record PredictionArguments(
    decimal CurrentPrice,
    decimal ThresholdValue,
    string Direction,
    double DailyVolatility,
    int DaysToResolution,
    double DistanceInSigmas,
    double ModelProbability,
    double MarketImpliedProbability,
    decimal Volume24h,
    decimal Liquidity,
    int DaysOfPriceHistoryUsed);