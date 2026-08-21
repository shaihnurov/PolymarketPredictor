using Microsoft.EntityFrameworkCore;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Domain.Entities;

namespace PolymarketPredictor.Persistence.Repositories;

/// <summary>
/// Реализация <see cref="IPredictionRepository"/>
/// summary>
public class PredictionRepository(AppDbContext dbContext) : IPredictionRepository
{
    /// <inheritdoc />
    public Task<Prediction?> GetLatestAsync(Guid trackedMarketId, CancellationToken ct) =>
        dbContext.Predictions.AsNoTracking().Where(p => p.TrackedMarketId == trackedMarketId)
            .OrderByDescending(p => p.CreatedAt).FirstOrDefaultAsync(ct);

    /// <inheritdoc />
    public Task<List<Prediction>> GetHistoryAsync(Guid trackedMarketId, CancellationToken ct) =>
        dbContext.Predictions.AsNoTracking().Where(p => p.TrackedMarketId == trackedMarketId)
            .OrderBy(p => p.CreatedAt).ToListAsync(ct);

    /// <inheritdoc />
    /// <remarks>
    /// Группировка "последний прогноз на рынок" делается в памяти после выборки, а не через SQL GROUP BY
    /// EF Core не умеет надёжно транслировать "OrderByDescending().First() per group" в один SQL-запрос
    /// без оконных функций. Для масштаба этого проекта (десятки рынков, не миллионы) это приемлемо;
    /// при росте датасета — переписать на raw SQL с ROW_NUMBER() OVER (PARTITION BY ...)
    /// </remarks>
    public async Task<Dictionary<Guid, Prediction>> GetLatestByMarketIdsAsync(IReadOnlyCollection<Guid> trackedMarketIds, CancellationToken ct)
    {
        var predictions = await dbContext.Predictions.AsNoTracking().Where(p => trackedMarketIds.Contains(p.TrackedMarketId))
            .OrderByDescending(p => p.CreatedAt).ToListAsync(ct);

        return predictions.GroupBy(p => p.TrackedMarketId).ToDictionary(g => g.Key, g => g.First());
    }

    /// <inheritdoc />
    public void Add(Prediction prediction) => dbContext.Predictions.Add(prediction);
}