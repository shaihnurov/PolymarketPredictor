using Microsoft.EntityFrameworkCore;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Domain.Entities;
using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.Persistence.Repositories;

/// <summary>
/// Реализация <see cref="IRawSnapshotRepository"/>
/// </summary>
public class RawSnapshotRepository(AppDbContext dbContext) : IRawSnapshotRepository
{
    /// <inheritdoc />
    public Task<RawSnapshot?> GetLatestAsync(Guid trackedMarketId, SourceType sourceType, CancellationToken ct) =>
        dbContext.RawSnapshots.AsNoTracking().Where(s => s.TrackedMarketId == trackedMarketId && s.SourceType == sourceType)
            .OrderByDescending(s => s.FetchedAt).FirstOrDefaultAsync(ct);

    /// <inheritdoc />
    public void Add(RawSnapshot snapshot) => dbContext.RawSnapshots.Add(snapshot);
}