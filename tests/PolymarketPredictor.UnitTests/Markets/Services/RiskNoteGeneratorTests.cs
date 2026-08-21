using FluentAssertions;
using PolymarketPredictor.Application.Markets.Services;

namespace PolymarketPredictor.UnitTests.Markets.Services;

/// <summary>
/// Юнит-тесты <see cref="RiskNoteGenerator"/>
/// </summary>
public class RiskNoteGeneratorTests
{
    [Fact]
    public void Generate_ReturnsEmptyList_WhenNoRuleTriggers()
    {
        var notes = RiskNoteGenerator.Generate(modelProbability: 0.5, marketImpliedProbability: 0.52, daysToResolution: 60, 
            dailyVolatility: 0.02, volume24h: 20_000m);

        notes.Should().BeEmpty();
    }

    [Fact]
    public void Generate_AddsDivergenceNote_WhenModelAndMarketDisagreeSignificantly()
    {
        var notes = RiskNoteGenerator.Generate(modelProbability: 0.8, marketImpliedProbability: 0.5, daysToResolution: 60, 
            dailyVolatility: 0.02, volume24h: 20_000m);

        notes.Should().ContainSingle();
    }

    [Fact]
    public void Generate_AddsAllApplicableNotes_WhenAllRulesTrigger()
    {
        var notes = RiskNoteGenerator.Generate(modelProbability: 0.9, marketImpliedProbability: 0.2, daysToResolution: 1, 
            dailyVolatility: 0.08, volume24h: 1_000m);

        notes.Should().HaveCount(4);
    }
}