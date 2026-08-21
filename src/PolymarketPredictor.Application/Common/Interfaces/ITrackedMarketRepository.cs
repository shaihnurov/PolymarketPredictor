using PolymarketPredictor.Domain.Entities;
using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.Application.Common.Interfaces;

/// <summary>
/// Репозиторий сущности <see cref="TrackedMarket"/>
/// </summary>
public interface ITrackedMarketRepository
{
    /// <summary>
    /// Найти рынок по внутреннему идентификатору
    /// </summary>
    /// <param name="id">Идентификатор рынка в БД</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Рынок или null, если не найден</returns>
    Task<TrackedMarket?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Получить набор conditionId, которые уже отслеживаются, из переданного списка — одним запросом,
    /// без обращения к БД по одному conditionId за раз (используется при пакетном сидировании/синке)
    /// </summary>
    /// <param name="conditionIds">conditionId, которые нужно проверить на существование</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Подмножество переданных conditionId, которые уже есть в БД</returns>
    Task<HashSet<string>> GetExistingConditionIdsAsync(IReadOnlyCollection<string> conditionIds, CancellationToken ct);

    /// <summary>
    /// Получить список всех отслеживаемых рынков без дочерних коллекций
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    Task<List<TrackedMarket>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Получить идентификаторы всех открытых (ещё не резолвленных) рынков — для планировщика синка
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    Task<List<Guid>> GetOpenMarketIdsAsync(CancellationToken ct);

    /// <summary>
    /// Получить все резолвленные рынки — нужно для расчёта Brier score
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    Task<List<TrackedMarket>> GetResolvedMarketsAsync(CancellationToken ct);

    /// <summary>
    /// Добавить новый отслеживаемый рынок
    /// </summary>
    /// <param name="market">Новая сущность рынка</param>
    void Add(TrackedMarket market);
}