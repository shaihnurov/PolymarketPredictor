using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Application.Common.Models.CoinGecko;
using PolymarketPredictor.Application.Common.Models.Polymarket;
using PolymarketPredictor.Application.Markets.Commands;
using PolymarketPredictor.Domain.Entities;
using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.UnitTests.Markets.Commands;

/// <summary>
/// Юнит-тесты <see cref="SyncMarketCommandHandler"/> на моках всех зависимостей
/// </summary>
public class SyncMarketCommandHandlerTests
{
    private readonly ITrackedMarketRepository _marketRepository = Substitute.For<ITrackedMarketRepository>();
    private readonly IRawSnapshotRepository _rawSnapshotRepository = Substitute.For<IRawSnapshotRepository>();
    private readonly INormalizedIndicatorRepository _indicatorRepository = Substitute.For<INormalizedIndicatorRepository>();
    private readonly IPredictionRepository _predictionRepository = Substitute.For<IPredictionRepository>();
    private readonly IPolymarketClient _polymarketClient = Substitute.For<IPolymarketClient>();
    private readonly ICoinGeckoClient _coinGeckoClient = Substitute.For<ICoinGeckoClient>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly SyncMarketCommandHandler _sut;

    public SyncMarketCommandHandlerTests()
    {
        _sut = new SyncMarketCommandHandler(_marketRepository, _rawSnapshotRepository, _indicatorRepository, _predictionRepository,
            _polymarketClient, _coinGeckoClient, _unitOfWork, Substitute.For<ILogger<SyncMarketCommandHandler>>());
    }

    private static TrackedMarket CreateOpenMarket() => new()
    {
        Id = Guid.NewGuid(),
        PolymarketConditionId = "0xabc",
        Question = "Will BTC be above $150,000 on Dec 31, 2026?",
        AssetSymbol = "BTC",
        CoinGeckoAssetId = "bitcoin",
        ThresholdValue = 150_000m,
        Direction = ThresholdDirection.Above,
        ResolutionDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
        Status = MarketStatus.Open,
        CreatedAt = DateTimeOffset.UtcNow
    };

    [Fact]
    public async Task Handle_DoesNothing_WhenMarketNotFound()
    {
        var marketId = Guid.NewGuid();
        _marketRepository.GetByIdAsync(marketId, Arg.Any<CancellationToken>()).Returns((TrackedMarket?)null);

        await _sut.Handle(new SyncMarketCommand(marketId), CancellationToken.None);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MarksMarketResolved_WhenPolymarketReportsClosed()
    {
        var market = CreateOpenMarket();
        _marketRepository.GetByIdAsync(market.Id, Arg.Any<CancellationToken>()).Returns(market);
        _polymarketClient.GetMarketByConditionIdAsync(market.PolymarketConditionId, Arg.Any<CancellationToken>()).Returns(new GammaMarketDto(market.PolymarketConditionId, 
            market.Question, Active: false, Closed: true, Volume24hr: 1000m, Liquidity: 2000m, YesPrice: 1.0));

        await _sut.Handle(new SyncMarketCommand(market.Id), CancellationToken.None);

        market.Status.Should().Be(MarketStatus.Resolved);
        market.ActualOutcome.Should().BeTrue();
        await _coinGeckoClient.DidNotReceive().GetCurrentPriceAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        _predictionRepository.DidNotReceive().Add(Arg.Any<Prediction>());
    }

    [Fact]
    public async Task Handle_SavesRawSnapshotsIndicatorAndPrediction_WhenMarketStillOpen()
    {
        var market = CreateOpenMarket();
        _marketRepository.GetByIdAsync(market.Id, Arg.Any<CancellationToken>()).Returns(market);
        _polymarketClient.GetMarketByConditionIdAsync(market.PolymarketConditionId, Arg.Any<CancellationToken>()).Returns(new GammaMarketDto(market.PolymarketConditionId, 
            market.Question, Active: true, Closed: false, Volume24hr: 40_000m, Liquidity: 60_000m, YesPrice: 0.25));

        _coinGeckoClient.GetCurrentPriceAsync(market.CoinGeckoAssetId, Arg.Any<CancellationToken>()).Returns(140_000m);

        _coinGeckoClient.GetDailyPriceHistoryAsync(market.CoinGeckoAssetId, 30, Arg.Any<CancellationToken>()).Returns(
        [
            new(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)), 138_000m),
            new(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), 139_000m),
            new(DateOnly.FromDateTime(DateTime.UtcNow), 140_000m),
        ]);

        await _sut.Handle(new SyncMarketCommand(market.Id), CancellationToken.None);

        _rawSnapshotRepository.Received(2).Add(Arg.Any<RawSnapshot>());
        _indicatorRepository.Received(1).Add(Arg.Any<NormalizedIndicator>());
        _predictionRepository.Received(1).Add(Arg.Any<Prediction>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}