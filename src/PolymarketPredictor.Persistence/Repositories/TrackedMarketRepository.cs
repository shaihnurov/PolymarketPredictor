using Microsoft.EntityFrameworkCore;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Domain.Entities;
using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.Persistence.Repositories;

/// <summary>
/// Реализация <see cref="ITrackedMarketRepository"/>
/// </summary>
public class TrackedMarketRepository(AppDbContext dbContext) : ITrackedMarketRepository
{
    /// <inheritdoc />
    public Task<TrackedMarket?> GetByIdAsync(Guid id, CancellationToken ct) =>
        dbContext.TrackedMarkets.FirstOrDefaultAsync(m => m.Id == id, ct);

    /// <inheritdoc />
    /// <remarks>Только для проверки существования — без трекинга</remarks>
    public async Task<HashSet<string>> GetExistingConditionIdsAsync(IReadOnlyCollection<string> conditionIds, CancellationToken ct)
    {
        var existing = await dbContext.TrackedMarkets.AsNoTracking().Where(m => conditionIds.Contains(m.PolymarketConditionId))
            .Select(m => m.PolymarketConditionId).ToListAsync(ct);

        return [.. existing];
    }

    /// <inheritdoc />
    public Task<List<TrackedMarket>> GetAllAsync(CancellationToken ct) =>
        dbContext.TrackedMarkets.AsNoTracking().OrderByDescending(m => m.CreatedAt).ToListAsync(ct);

    /// <inheritdoc />
    /// <remarks>Только список Id для планировщика — без трекинга и без лишних данных</remarks>
    public Task<List<Guid>> GetOpenMarketIdsAsync(CancellationToken ct) =>
        dbContext.TrackedMarkets.AsNoTracking().Where(m => m.Status == MarketStatus.Open).Select(m => m.Id).ToListAsync(ct);

    /// <inheritdoc />
    public void Add(TrackedMarket market) => dbContext.TrackedMarkets.Add(market);
}