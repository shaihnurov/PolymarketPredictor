using FluentAssertions;
using NSubstitute;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Application.Markets.Commands;
using PolymarketPredictor.Application.Markets.Seed;
using PolymarketPredictor.Domain.Entities;

namespace PolymarketPredictor.UnitTests.Markets.Commands;

/// <summary>
/// Юнит-тесты <see cref="SeedMarketsCommandHandler"/>
/// </summary>
public class SeedMarketsCommandHandlerTests
{
    private readonly ITrackedMarketRepository _marketRepository = Substitute.For<ITrackedMarketRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly SeedMarketsCommandHandler _sut;

    public SeedMarketsCommandHandlerTests()
    {
        _sut = new SeedMarketsCommandHandler(_marketRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_AddsAllMarkets_WhenNoneExistYet()
    {
        _marketRepository.GetExistingConditionIdsAsync(Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>()).Returns([]);

        var addedCount = await _sut.Handle(new SeedMarketsCommand(), CancellationToken.None);

        addedCount.Should().Be(MarketSeedList.Items.Count);
        _marketRepository.Received(MarketSeedList.Items.Count).Add(Arg.Any<TrackedMarket>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SkipsAlreadyTrackedMarkets_AndDoesNotSave_WhenAllAlreadyExist()
    {
        var allConditionIds = MarketSeedList.Items.Select(x => x.ConditionId).ToHashSet();
        _marketRepository.GetExistingConditionIdsAsync(Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>()).Returns(allConditionIds);

        var addedCount = await _sut.Handle(new SeedMarketsCommand(), CancellationToken.None);

        addedCount.Should().Be(0);
        _marketRepository.DidNotReceive().Add(Arg.Any<TrackedMarket>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AddsOnlyNewMarkets_WhenSomeAlreadyExist()
    {
        var oneExistingId = MarketSeedList.Items[0].ConditionId;
        _marketRepository.GetExistingConditionIdsAsync(Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>()).Returns([oneExistingId]);

        var addedCount = await _sut.Handle(new SeedMarketsCommand(), CancellationToken.None);

        addedCount.Should().Be(MarketSeedList.Items.Count - 1);
    }
}