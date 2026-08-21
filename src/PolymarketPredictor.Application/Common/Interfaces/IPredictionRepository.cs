using PolymarketPredictor.Domain.Entities;

namespace PolymarketPredictor.Application.Common.Interfaces;

/// <summary>
/// Репозиторий сущности <see cref="Prediction"/>
/// </summary>
public interface IPredictionRepository
{
    /// <summary>
    /// Получить самый свежий прогноз конкретного рынка
    /// </summary>
    /// <param name="trackedMarketId">Идентификатор рынка</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Самый свежий прогноз или null, если прогнозов ещё не было</returns>
    Task<Prediction?> GetLatestAsync(Guid trackedMarketId, CancellationToken ct);

    /// <summary>
    /// Получить полную историю прогнозов рынка, от старых к новым
    /// </summary>
    /// <param name="trackedMarketId">Идентификатор рынка</param>
    /// <param name="ct">Токен отмены</param>
    Task<List<Prediction>> GetHistoryAsync(Guid trackedMarketId, CancellationToken ct);

    /// <summary>
    /// Получить последний прогноз для каждого из перечисленных рынков одним запросом
    /// используется списком рынков, чтобы не ходить в БД по одному прогнозу на рынок (N+1)
    /// </summary>
    /// <param name="trackedMarketIds">Идентификаторы рынков, для которых нужен последний прогноз</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Словарь "идентификатор рынка -> его последний прогноз". Рынки без прогнозов в словарь не попадают</returns>
    Task<Dictionary<Guid, Prediction>> GetLatestByMarketIdsAsync(IReadOnlyCollection<Guid> trackedMarketIds, CancellationToken ct);

    /// <summary>
    /// Добавить новый прогноз. Таблица append-only — обновление существующих строк не предусмотрено
    /// </summary>
    /// <param name="prediction">Новая сущность прогноза</param>
    void Add(Prediction prediction);
}