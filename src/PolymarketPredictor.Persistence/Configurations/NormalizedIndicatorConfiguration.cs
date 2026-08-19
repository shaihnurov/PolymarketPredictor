using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolymarketPredictor.Domain.Entities;

namespace PolymarketPredictor.Persistence.Configurations;

/// <summary>
/// Fluent-конфигурация таблицы <see cref="NormalizedIndicator"/>
/// </summary>
public class NormalizedIndicatorConfiguration : IEntityTypeConfiguration<NormalizedIndicator>
{
    /// <summary>
    /// Настраивает маппинг сущности <see cref="NormalizedIndicator"/> на таблицу normalized_indicators
    /// </summary>
    /// <param name="builder">Строитель конфигурации для этой сущности</param>
    public void Configure(EntityTypeBuilder<NormalizedIndicator> builder)
    {
        builder.ToTable("normalized_indicators");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CurrentAssetPrice).HasPrecision(18, 4);
        builder.Property(x => x.Volume24h).HasPrecision(18, 4);
        builder.Property(x => x.Liquidity).HasPrecision(18, 4);

        builder.HasIndex(x => new { x.TrackedMarketId, x.ComputedAt });

        // Restrict а не Cascade: NormalizedIndicator не должен удаляться каскадом через Prediction
        // это исторические данные, на которые прогноз ссылается, они не менее ценны, чем сам прогноз
        builder.HasMany(x => x.Predictions).WithOne(x => x.NormalizedIndicator)
            .HasForeignKey(x => x.NormalizedIndicatorId).OnDelete(DeleteBehavior.Restrict);
    }
}