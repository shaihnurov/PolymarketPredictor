using FluentAssertions;
using PolymarketPredictor.Application.Markets.Services;

namespace PolymarketPredictor.UnitTests.Markets.Services;

/// <summary>
/// Юнит-тесты <see cref="ConfidenceCalculator"/>
/// </summary>
public class ConfidenceCalculatorTests
{
    [Fact]
    public void Calculate_ReturnsMaxConfidence_WhenAllComponentsAtOrAboveCap()
    {
        var confidence = ConfidenceCalculator.Calculate(volume24h: 100_000m, liquidity: 100_000m, daysOfPriceHistoryUsed: 60, distanceInSigmas: 5.0);

        confidence.Should().BeApproximately(1.0, precision: 0.0001);
    }

    [Fact]
    public void Calculate_ReturnsLowConfidence_WhenAllComponentsAreZero()
    {
        var confidence = ConfidenceCalculator.Calculate(volume24h: 0m, liquidity: 0m, daysOfPriceHistoryUsed: 0, distanceInSigmas: 0.0);

        confidence.Should().Be(0.0);
    }

    [Fact]
    public void Calculate_UsesAbsoluteValue_ForNegativeDistanceInSigmas()
    {
        var confidencePositive = ConfidenceCalculator.Calculate(10_000m, 10_000m, 30, distanceInSigmas: 1.5);
        var confidenceNegative = ConfidenceCalculator.Calculate(10_000m, 10_000m, 30, distanceInSigmas: -1.5);

        confidencePositive.Should().BeApproximately(confidenceNegative, precision: 0.0001);
    }
}