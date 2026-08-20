using PolymarketPredictor.Domain.Entities;

namespace PolymarketPredictor.Application.Common.Interfaces;

/// <summary>
/// Репозиторий сущности <see cref="NormalizedIndicator"/>
/// </summary>
public interface INormalizedIndicatorRepository
{
    /// <summary>
    /// Получить самый свежий набор нормализованных показателей конкретного рынка
    /// </summary>
    /// <param name="trackedMarketId">Идентификатор рынка</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Самые свежие показатели или null, если ещё не считались</returns>
    Task<NormalizedIndicator?> GetLatestAsync(Guid trackedMarketId, CancellationToken ct);

    /// <summary>
    /// Добавить новый набор нормализованных показателей
    /// </summary>
    /// <param name="indicator">Новая сущность показателей</param>
    void Add(NormalizedIndicator indicator);
}