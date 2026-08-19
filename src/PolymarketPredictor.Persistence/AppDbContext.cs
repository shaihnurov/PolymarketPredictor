using Microsoft.EntityFrameworkCore;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Domain.Entities;

namespace PolymarketPredictor.Persistence;

/// <summary>
/// DbContext
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IApplicationDbContext
{
    /// <inheritdoc/>
    public DbSet<TrackedMarket> TrackedMarkets => Set<TrackedMarket>();

    /// <inheritdoc/>
    public DbSet<RawSnapshot> RawSnapshots => Set<RawSnapshot>();

    /// <inheritdoc/>
    public DbSet<NormalizedIndicator> NormalizedIndicators => Set<NormalizedIndicator>();

    /// <inheritdoc/>
    public DbSet<Prediction> Predictions => Set<Prediction>();

    /// <summary>
    /// Применяет все конфигурации <see cref="IEntityTypeConfiguration{TEntity}"/>
    /// из текущей сборки (папка Configurations) при построении модели
    /// </summary>
    /// <param name="modelBuilder">Строитель модели EF Core</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}