using Microsoft.EntityFrameworkCore;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Domain.Entities;

namespace PolymarketPredictor.Persistence.Repositories;

/// <summary>
/// Реализация <see cref="INormalizedIndicatorRepository"/>
/// </summary>
public class NormalizedIndicatorRepository(AppDbContext dbContext) : INormalizedIndicatorRepository
{
    /// <inheritdoc />
    public Task<NormalizedIndicator?> GetLatestAsync(Guid trackedMarketId, CancellationToken ct) =>
        dbContext.NormalizedIndicators.AsNoTracking().Where(i => i.TrackedMarketId == trackedMarketId)
            .OrderByDescending(i => i.ComputedAt).FirstOrDefaultAsync(ct);

    /// <inheritdoc />
    public void Add(NormalizedIndicator indicator) => dbContext.NormalizedIndicators.Add(indicator);
}