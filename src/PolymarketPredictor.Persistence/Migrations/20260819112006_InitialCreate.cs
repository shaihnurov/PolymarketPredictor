using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolymarketPredictor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tracked_markets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PolymarketConditionId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Question = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    AssetSymbol = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    CoinGeckoAssetId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ThresholdValue = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Direction = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    ResolutionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    ActualOutcome = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tracked_markets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "normalized_indicators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrackedMarketId = table.Column<Guid>(type: "uuid", nullable: false),
                    PolymarketImpliedProbability = table.Column<double>(type: "double precision", nullable: false),
                    CurrentAssetPrice = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    DailyVolatility30d = table.Column<double>(type: "double precision", nullable: false),
                    DaysToResolution = table.Column<int>(type: "integer", nullable: false),
                    DistanceInSigmas = table.Column<double>(type: "double precision", nullable: false),
                    Volume24h = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Liquidity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    DaysOfPriceHistoryUsed = table.Column<int>(type: "integer", nullable: false),
                    ComputedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_normalized_indicators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_normalized_indicators_tracked_markets_TrackedMarketId",
                        column: x => x.TrackedMarketId,
                        principalTable: "tracked_markets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "raw_snapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrackedMarketId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceType = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    RawPayload = table.Column<string>(type: "jsonb", nullable: false),
                    FetchedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_raw_snapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_raw_snapshots_tracked_markets_TrackedMarketId",
                        column: x => x.TrackedMarketId,
                        principalTable: "tracked_markets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "predictions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrackedMarketId = table.Column<Guid>(type: "uuid", nullable: false),
                    NormalizedIndicatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    PredictedProbability = table.Column<double>(type: "double precision", nullable: false),
                    ConfidenceScore = table.Column<double>(type: "double precision", nullable: false),
                    RiskNotes = table.Column<List<string>>(type: "text[]", nullable: false),
                    ArgumentsJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_predictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_predictions_normalized_indicators_NormalizedIndicatorId",
                        column: x => x.NormalizedIndicatorId,
                        principalTable: "normalized_indicators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_predictions_tracked_markets_TrackedMarketId",
                        column: x => x.TrackedMarketId,
                        principalTable: "tracked_markets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_normalized_indicators_TrackedMarketId_ComputedAt",
                table: "normalized_indicators",
                columns: new[] { "TrackedMarketId", "ComputedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_predictions_NormalizedIndicatorId",
                table: "predictions",
                column: "NormalizedIndicatorId");

            migrationBuilder.CreateIndex(
                name: "IX_predictions_TrackedMarketId_CreatedAt",
                table: "predictions",
                columns: new[] { "TrackedMarketId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_raw_snapshots_TrackedMarketId_SourceType_FetchedAt",
                table: "raw_snapshots",
                columns: new[] { "TrackedMarketId", "SourceType", "FetchedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_tracked_markets_PolymarketConditionId",
                table: "tracked_markets",
                column: "PolymarketConditionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "predictions");

            migrationBuilder.DropTable(
                name: "raw_snapshots");

            migrationBuilder.DropTable(
                name: "normalized_indicators");

            migrationBuilder.DropTable(
                name: "tracked_markets");
        }
    }
}
