using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolymarketPredictor.Domain.Entities;

namespace PolymarketPredictor.Persistence.Configurations;

/// <summary>
/// Fluent-конфигурация таблицы <see cref="RawSnapshot"/>
/// </summary>
public class RawSnapshotConfiguration : IEntityTypeConfiguration<RawSnapshot>
{
    /// <summary>
    /// Настраивает маппинг сущности <see cref="RawSnapshot"/> на таблицу raw_snapshots
    /// </summary>
    /// <param name="builder">Строитель конфигурации для этой сущности</param>
    public void Configure(EntityTypeBuilder<RawSnapshot> builder)
    {
        builder.ToTable("raw_snapshots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SourceType).HasConversion<string>().HasMaxLength(16);

        builder.Property(x => x.RawPayload).HasColumnType("jsonb").IsRequired();

        builder.HasIndex(x => new { x.TrackedMarketId, x.SourceType, x.FetchedAt });
    }
}