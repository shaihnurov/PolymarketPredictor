namespace PolymarketPredictor.Application.Markets.Dtos;

/// <summary>
/// Одна точка истории прогнозов рынка — для графика "прогноз со временем" на экране деталей
/// </summary>
/// <param name="PredictedProbability">Вероятность, посчитанная моделью на этот момент времени</param>
/// <param name="ConfidenceScore">Уверенность модели в этот момент времени</param>
/// <param name="CreatedAt">Момент, когда этот прогноз был посчитан</param>
public sealed record PredictionHistoryItemDto(double PredictedProbability, double ConfidenceScore, DateTimeOffset CreatedAt);