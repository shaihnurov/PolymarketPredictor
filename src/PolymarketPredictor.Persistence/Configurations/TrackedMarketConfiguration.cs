using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolymarketPredictor.Domain.Entities;

namespace PolymarketPredictor.Persistence.Configurations;

/// <summary>
/// Fluent-конфигурация таблицы <see cref="TrackedMarket"/>
/// </summary>
public class TrackedMarketConfiguration : IEntityTypeConfiguration<TrackedMarket>
{
    /// <summary>
    /// Настраивает маппинг сущности <see cref="TrackedMarket"/> на таблицу tracked_markets
    /// </summary>
    /// <param name="builder">Строитель конфигурации для этой сущности</param>
    public void Configure(EntityTypeBuilder<TrackedMarket> builder)
    {
        builder.ToTable("tracked_markets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PolymarketConditionId).HasMaxLength(128).IsRequired();

        // conditionId уникален в рамках Polymarket — не должно быть двух TrackedMarket на один и тот же рынок
        builder.HasIndex(x => x.PolymarketConditionId).IsUnique();

        builder.Property(x => x.Question).HasMaxLength(1024).IsRequired();
        builder.Property(x => x.AssetSymbol).HasMaxLength(16).IsRequired();
        builder.Property(x => x.CoinGeckoAssetId).HasMaxLength(64).IsRequired();

        builder.Property(x => x.ThresholdValue).HasPrecision(18, 4);

        builder.Property(x => x.Direction).HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(16);

        builder.HasMany(x => x.RawSnapshots).WithOne(x => x.TrackedMarket)
            .HasForeignKey(x => x.TrackedMarketId).OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.NormalizedIndicators).WithOne(x => x.TrackedMarket)
            .HasForeignKey(x => x.TrackedMarketId).OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Predictions).WithOne(x => x.TrackedMarket)
            .HasForeignKey(x => x.TrackedMarketId).OnDelete(DeleteBehavior.Cascade);
    }
}