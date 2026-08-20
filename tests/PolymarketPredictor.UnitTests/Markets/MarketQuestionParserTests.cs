using FluentAssertions;
using PolymarketPredictor.Application.Markets.Parsing;
using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.UnitTests.Markets;

/// <summary>
/// Юнит-тесты <see cref="MarketQuestionParser"/>
/// </summary>
public class MarketQuestionParserTests
{
    [Theory]
    [InlineData("Will Bitcoin reach $150,000 by December 31, 2026?", "BITCOIN", 150000, ThresholdDirection.Above)]
    [InlineData("Will BTC be above $100000 by Dec 31?", "BTC", 100000, ThresholdDirection.Above)]
    [InlineData("Will Ethereum drop below $2,000 before January 1, 2027?", "ETHEREUM", 2000, ThresholdDirection.Below)]
    public void TryParse_RecognizesSupportedPhrasings(string question, string expectedAsset, decimal expectedThreshold, ThresholdDirection expectedDirection)
    {
        var result = MarketQuestionParser.TryParse(question);

        result.Should().NotBeNull();
        result!.AssetSymbol.Should().Be(expectedAsset);
        result.ThresholdValue.Should().Be(expectedThreshold);
        result.Direction.Should().Be(expectedDirection);
    }

    [Fact]
    public void TryParse_ReturnsNull_ForUnsupportedPhrasing()
    {
        var result = MarketQuestionParser.TryParse("Who will win the election in 2028?");

        result.Should().BeNull();
    }
}