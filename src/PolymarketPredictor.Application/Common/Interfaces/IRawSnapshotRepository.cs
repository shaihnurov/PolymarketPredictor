using PolymarketPredictor.Domain.Entities;
using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.Application.Common.Interfaces;

/// <summary>
/// Репозиторий сущности <see cref="RawSnapshot"/>
/// </summary>
public interface IRawSnapshotRepository
{
    /// <summary>
    /// Получить самый свежий снимок конкретного рынка из конкретного источника
    /// </summary>
    /// <param name="trackedMarketId">Идентификатор рынка</param>
    /// <param name="sourceType">Источник данных</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Самый свежий снимок или null, если снимков ещё не было</returns>
    Task<RawSnapshot?> GetLatestAsync(Guid trackedMarketId, SourceType sourceType, CancellationToken ct);

    /// <summary>
    /// Добавить новый сырой снимок
    /// </summary>
    /// <param name="snapshot">Новая сущность снимка</param>
    void Add(RawSnapshot snapshot);
}