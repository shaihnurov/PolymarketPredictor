using Microsoft.EntityFrameworkCore;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Domain.Entities;

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
    public Task<TrackedMarket?> GetByConditionIdAsync(string polymarketConditionId, CancellationToken ct) =>
        dbContext.TrackedMarkets.AsNoTracking().FirstOrDefaultAsync(m => m.PolymarketConditionId == polymarketConditionId, ct);

    /// <inheritdoc />
    public Task<List<TrackedMarket>> GetAllAsync(CancellationToken ct) =>
        dbContext.TrackedMarkets.AsNoTracking().OrderByDescending(m => m.CreatedAt).ToListAsync(ct);

    /// <inheritdoc />
    public void Add(TrackedMarket market) => dbContext.TrackedMarkets.Add(market);
}