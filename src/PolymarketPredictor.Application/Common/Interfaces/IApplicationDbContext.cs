using Microsoft.EntityFrameworkCore;
using PolymarketPredictor.Domain.Entities;

namespace PolymarketPredictor.Application.Common.Interfaces;

/// <summary>
/// Узкий контракт доступа к БД для Application-слоя
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Отслеживаемые рынки
    /// </summary>
    DbSet<TrackedMarket> TrackedMarkets { get; }

    /// <summary>
    /// Сырые снимки данных источников
    /// </summary>
    DbSet<RawSnapshot> RawSnapshots { get; }

    /// <summary>
    /// Нормализованные показатели по циклам синка
    /// </summary>
    DbSet<NormalizedIndicator> NormalizedIndicators { get; }

    /// <summary>
    /// История прогнозов
    /// </summary>
    DbSet<Prediction> Predictions { get; }

    /// <summary>
    /// Сохранить накопленные изменения в БД
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns>Количество затронутых строк</returns>
    Task<int> SaveChangesAsync(CancellationToken ct);
}