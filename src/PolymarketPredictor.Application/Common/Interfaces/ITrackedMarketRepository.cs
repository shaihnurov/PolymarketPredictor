using PolymarketPredictor.Domain.Entities;

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
    /// Найти рынок по conditionId Polymarket — используется при синке, чтобы не создавать дубликаты
    /// </summary>
    /// <param name="polymarketConditionId">conditionId рынка в Polymarket</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Рынок или null, если ещё не отслеживается</returns>
    Task<TrackedMarket?> GetByConditionIdAsync(string polymarketConditionId, CancellationToken ct);

    /// <summary>
    /// Получить список всех отслеживаемых рынков без дочерних коллекций
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    Task<List<TrackedMarket>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Добавить новый отслеживаемый рынок
    /// </summary>
    /// <param name="market">Новая сущность рынка</param>
    void Add(TrackedMarket market);
}