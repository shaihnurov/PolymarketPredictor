using PolymarketPredictor.Application.Common.Interfaces;

namespace PolymarketPredictor.Persistence;

/// <summary>
/// Реализация <see cref="IUnitOfWork"/> поверх <see cref="AppDbContext"/>
/// </summary>
public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken ct) => dbContext.SaveChangesAsync(ct);
}