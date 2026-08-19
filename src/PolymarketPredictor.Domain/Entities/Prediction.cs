namespace PolymarketPredictor.Domain.Entities;

/// <summary>
/// Один цикл прогноза по рынку. Таблица append-only: история прогнозов по рынку
/// это все строки с данным TrackedMarketId, упорядоченные по CreatedAt. Ничего не перезаписывается.
/// </summary>
public class Prediction
{
    /// <summary>
    /// Первичный ключ
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Ссылка на рынок, по которому сделан прогноз
    /// </summary>
    public Guid TrackedMarketId { get; set; }

    /// <summary>
    /// Навигационное свойство к рынку
    /// </summary>
    public TrackedMarket TrackedMarket { get; set; } = default!;

    /// <summary>
    /// Показатели, из которых детерминированно посчитан этот прогноз
    /// </summary>
    public Guid NormalizedIndicatorId { get; set; }

    /// <summary>
    /// Навигационное свойство к показателям, на основе которых сделан прогноз
    /// </summary>
    public NormalizedIndicator NormalizedIndicator { get; set; } = default!;

    /// <summary>
    /// Вероятность модели, что пороговое условие сбудется к дате резолюции, 0..1
    /// </summary>
    public double PredictedProbability { get; set; }

    /// <summary>
    /// Уверенность модели в этом конкретном прогнозе, 0..1 (не путать с самой вероятностью)
    /// </summary>
    public double ConfidenceScore { get; set; }

    /// <summary>
    /// Человекочитаемые причины, почему прогноз может не сбыться
    /// </summary>
    public List<string> RiskNotes { get; set; } = [];

    /// <summary>
    /// JSON-разбивка аргументов прогноза по факторам с весами и вкладом каждого
    /// то, что карточка прогноза показывает пользователю как "почему именно такая цифра"
    /// </summary>
    public string ArgumentsJson { get; set; } = default!;

    /// <summary>
    /// Момент, когда этот прогноз был посчитан и сохранён (UTC)
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
}