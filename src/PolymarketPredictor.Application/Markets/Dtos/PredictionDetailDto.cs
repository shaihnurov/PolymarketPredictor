namespace PolymarketPredictor.Application.Markets.Dtos;

/// <summary>
/// Детали одного прогноза вместе с полной разбивкой аргументов, для карточки прогноза
/// </summary>
/// <param name="PredictedProbability">Вероятность, посчитанная моделью</param>
/// <param name="ConfidenceScore">Уверенность модели в этом прогнозе</param>
/// <param name="RiskNotes">Причины, почему прогноз может не сбыться</param>
/// <param name="Arguments">Полная разбивка входных данных, из которых посчитан прогноз</param>
/// <param name="CreatedAt">Момент, когда прогноз был посчитан</param>
public sealed record PredictionDetailDto(
    double PredictedProbability,
    double ConfidenceScore,
    IReadOnlyList<string> RiskNotes,
    PredictionArguments Arguments,
    DateTimeOffset CreatedAt);