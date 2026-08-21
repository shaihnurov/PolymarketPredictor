using FluentAssertions;
using PolymarketPredictor.Application.Common.Models.CoinGecko;
using PolymarketPredictor.Application.Markets.Services;

namespace PolymarketPredictor.UnitTests.Markets.Services;

/// <summary>
/// Юнит-тесты <see cref="VolatilityCalculator"/>
/// </summary>
public class VolatilityCalculatorTests
{
    [Fact]
    public void CalculateDailyVolatility_ReturnsZero_ForConstantPrice()
    {
        var history = new List<DailyPricePoint>
        {
            new(new DateOnly(2026, 1, 1), 100_000m),
            new(new DateOnly(2026, 1, 2), 100_000m),
            new(new DateOnly(2026, 1, 3), 100_000m),
        };

        var volatility = VolatilityCalculator.CalculateDailyVolatility(history);

        volatility.Should().Be(0.0);
    }

    [Fact]
    public void CalculateDailyVolatility_ReturnsPositiveValue_ForFluctuatingPrice()
    {
        var history = new List<DailyPricePoint>
        {
            new(new DateOnly(2026, 1, 1), 100_000m),
            new(new DateOnly(2026, 1, 2), 102_000m),
            new(new DateOnly(2026, 1, 3), 98_000m),
            new(new DateOnly(2026, 1, 4), 101_000m),
        };

        var volatility = VolatilityCalculator.CalculateDailyVolatility(history);

        volatility.Should().BeGreaterThan(0.0);
    }

    [Fact]
    public void CalculateDailyVolatility_Throws_WhenFewerThanTwoPoints()
    {
        var history = new List<DailyPricePoint> { new(new DateOnly(2026, 1, 1), 100_000m) };

        var act = () => VolatilityCalculator.CalculateDailyVolatility(history);

        act.Should().Throw<ArgumentException>();
    }
}