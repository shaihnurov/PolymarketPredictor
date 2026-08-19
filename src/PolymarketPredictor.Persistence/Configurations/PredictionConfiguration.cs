using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolymarketPredictor.Domain.Entities;

namespace PolymarketPredictor.Persistence.Configurations;

/// <summary>
/// Fluent-конфигурация таблицы <see cref="Prediction"/>
/// </summary>
public class PredictionConfiguration : IEntityTypeConfiguration<Prediction>
{
    /// <summary>
    /// Настраивает маппинг сущности <see cref="Prediction"/> на таблицу predictions
    /// </summary>
    /// <param name="builder">Строитель конфигурации для этой сущности</param>
    public void Configure(EntityTypeBuilder<Prediction> builder)
    {
        builder.ToTable("predictions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RiskNotes).HasColumnType("text[]");

        builder.Property(x => x.ArgumentsJson).HasColumnType("jsonb").IsRequired();

        builder.HasIndex(x => new { x.TrackedMarketId, x.CreatedAt });
    }
}