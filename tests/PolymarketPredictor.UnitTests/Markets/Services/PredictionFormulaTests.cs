using FluentAssertions;
using PolymarketPredictor.Application.Markets.Services;
using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.UnitTests.Markets.Services;

/// <summary>
/// Юнит-тесты <see cref="PredictionFormula"/>, включая граничные случаи
/// </summary>
public class PredictionFormulaTests
{
    [Fact]
    public void Calculate_ReturnsHighProbability_WhenPriceAlreadyAboveThreshold_AndDirectionIsAbove()
    {
        var result = PredictionFormula.Calculate(currentPrice: 160_000m, thresholdValue: 150_000m,
            direction: ThresholdDirection.Above, dailyVolatility: 0.03, daysToResolution: 30);

        result.Probability.Should().BeGreaterThan(0.5);
    }

    [Fact]
    public void Calculate_ReturnsLowProbability_WhenPriceFarBelowThreshold_AndDirectionIsAbove()
    {
        var result = PredictionFormula.Calculate(currentPrice: 100_000m, thresholdValue: 150_000m,
            direction: ThresholdDirection.Above, dailyVolatility: 0.02, daysToResolution: 30);

        result.Probability.Should().BeLessThan(0.5);
    }

    [Fact]
    public void Calculate_ReturnsHalf_WhenCurrentPriceEqualsThreshold()
    {
        var result = PredictionFormula.Calculate(currentPrice: 100_000m, thresholdValue: 100_000m,
            direction: ThresholdDirection.Above, dailyVolatility: 0.03, daysToResolution: 30);

        result.Probability.Should().BeApproximately(0.5, precision: 0.001);
        result.DistanceInSigmas.Should().BeApproximately(0.0, precision: 0.001);
    }

    [Fact]
    public void Calculate_ReturnsDeterministicOne_WhenDaysToResolutionIsZero_AndConditionAlreadyMet()
    {
        var result = PredictionFormula.Calculate(currentPrice: 160_000m, thresholdValue: 150_000m,
            direction: ThresholdDirection.Above, dailyVolatility: 0.03, daysToResolution: 0);

        result.Probability.Should().Be(1.0);
    }

    [Fact]
    public void Calculate_ReturnsDeterministicZero_WhenDaysToResolutionIsZero_AndConditionNotMet()
    {
        var result = PredictionFormula.Calculate(currentPrice: 140_000m, thresholdValue: 150_000m,
            direction: ThresholdDirection.Above, dailyVolatility: 0.03, daysToResolution: 0);

        result.Probability.Should().Be(0.0);
    }

    [Fact]
    public void Calculate_ReturnsDeterministicOne_WhenVolatilityIsZero_AndConditionAlreadyMet()
    {
        var result = PredictionFormula.Calculate(currentPrice: 160_000m, thresholdValue: 150_000m,
            direction: ThresholdDirection.Above, dailyVolatility: 0.0, daysToResolution: 10);

        result.Probability.Should().Be(1.0);
    }

    [Fact]
    public void Calculate_BelowDirection_MirrorsAboveDirection()
    {
        var above = PredictionFormula.Calculate(currentPrice: 100_000m, thresholdValue: 90_000m,
            direction: ThresholdDirection.Above, dailyVolatility: 0.03, daysToResolution: 30);

        var below = PredictionFormula.Calculate(currentPrice: 100_000m, thresholdValue: 90_000m,
            direction: ThresholdDirection.Below, dailyVolatility: 0.03, daysToResolution: 30);

        (above.Probability + below.Probability).Should().BeApproximately(1.0, precision: 0.0001);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Calculate_Throws_WhenCurrentPriceNotPositive(decimal invalidPrice)
    {
        var act = () => PredictionFormula.Calculate(invalidPrice, 100_000m, ThresholdDirection.Above, 0.03, 30);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}