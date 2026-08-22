using FluentAssertions;
using NSubstitute;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Application.Markets.Queries;
using PolymarketPredictor.Domain.Entities;
using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.UnitTests.Markets.Queries;

/// <summary>
/// Юнит-тесты <see cref="GetAccuracyStatsQueryHandler"/>
/// </summary>
public class GetAccuracyStatsQueryHandlerTests
{
    private readonly ITrackedMarketRepository _marketRepository = Substitute.For<ITrackedMarketRepository>();
    private readonly IPredictionRepository _predictionRepository = Substitute.For<IPredictionRepository>();
    private readonly GetAccuracyStatsQueryHandler _sut;

    public GetAccuracyStatsQueryHandlerTests()
    {
        _sut = new GetAccuracyStatsQueryHandler(_marketRepository, _predictionRepository);
    }

    private static TrackedMarket CreateResolvedMarket(bool actualOutcome) => new()
    {
        Id = Guid.NewGuid(),
        PolymarketConditionId = Guid.NewGuid().ToString(),
        Question = "test",
        AssetSymbol = "BTC",
        CoinGeckoAssetId = "bitcoin",
        ThresholdValue = 100_000m,
        Direction = ThresholdDirection.Above,
        ResolutionDate = DateOnly.FromDateTime(DateTime.UtcNow),
        Status = MarketStatus.Resolved,
        ActualOutcome = actualOutcome,
        CreatedAt = DateTimeOffset.UtcNow
    };

    private static Prediction CreatePrediction(Guid marketId, double predictedProbability) => new()
    {
        Id = Guid.NewGuid(),
        TrackedMarketId = marketId,
        NormalizedIndicatorId = Guid.NewGuid(),
        PredictedProbability = predictedProbability,
        ConfidenceScore = 0.5,
        RiskNotes = [],
        ArgumentsJson = "{}",
        CreatedAt = DateTimeOffset.UtcNow
    };

    [Fact]
    public async Task Handle_ReturnsNullScore_WhenNoResolvedMarkets()
    {
        _marketRepository.GetResolvedMarketsAsync(Arg.Any<CancellationToken>()).Returns([]);

        var result = await _sut.Handle(new GetAccuracyStatsQuery(), CancellationToken.None);

        result.ResolvedMarketsCount.Should().Be(0);
        result.BrierScore.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ComputesBrierScore_ForPerfectPredictions()
    {
        var marketTrue = CreateResolvedMarket(actualOutcome: true);
        var marketFalse = CreateResolvedMarket(actualOutcome: false);
        _marketRepository.GetResolvedMarketsAsync(Arg.Any<CancellationToken>()).Returns([marketTrue, marketFalse]);

        _predictionRepository.GetLatestByMarketIdsAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<Guid, Prediction>
            {
                [marketTrue.Id] = CreatePrediction(marketTrue.Id, predictedProbability: 1.0),
                [marketFalse.Id] = CreatePrediction(marketFalse.Id, predictedProbability: 0.0),
            });

        var result = await _sut.Handle(new GetAccuracyStatsQuery(), CancellationToken.None);

        result.BrierScore.Should().Be(0.0);
        result.MarketsIncludedInScore.Should().Be(2);
    }

    [Fact]
    public async Task Handle_ComputesBrierScore_ForWorstCasePredictions()
    {
        var market = CreateResolvedMarket(actualOutcome: true);
        _marketRepository.GetResolvedMarketsAsync(Arg.Any<CancellationToken>()).Returns([market]);

        _predictionRepository.GetLatestByMarketIdsAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<Guid, Prediction>
            {
                [market.Id] = CreatePrediction(market.Id, predictedProbability: 0.0),
            });

        var result = await _sut.Handle(new GetAccuracyStatsQuery(), CancellationToken.None);

        result.BrierScore.Should().Be(1.0);
    }

    [Fact]
    public async Task Handle_SkipsResolvedMarketsWithoutPrediction()
    {
        var marketWithPrediction = CreateResolvedMarket(actualOutcome: true);
        var marketWithoutPrediction = CreateResolvedMarket(actualOutcome: false);
        _marketRepository.GetResolvedMarketsAsync(Arg.Any<CancellationToken>()).Returns([marketWithPrediction, marketWithoutPrediction]);

        _predictionRepository.GetLatestByMarketIdsAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<Guid, Prediction>
            {
                [marketWithPrediction.Id] = CreatePrediction(marketWithPrediction.Id, predictedProbability: 1.0),
            });

        var result = await _sut.Handle(new GetAccuracyStatsQuery(), CancellationToken.None);

        result.ResolvedMarketsCount.Should().Be(2);
        result.MarketsIncludedInScore.Should().Be(1);
        result.BrierScore.Should().Be(0.0);
    }
}