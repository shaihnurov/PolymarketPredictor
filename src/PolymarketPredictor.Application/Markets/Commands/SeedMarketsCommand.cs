using MediatR;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Application.Markets.Seed;
using PolymarketPredictor.Domain.Entities;
using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.Application.Markets.Commands;

/// <summary>
/// Команда наполнения БД рынками из <see cref="MarketSeedList"/> — добавляет только те,
/// которых ещё нет (проверка по PolymarketConditionId), идемпотентна при повторном запуске
/// </summary>
public sealed record SeedMarketsCommand : IRequest<int>;

/// <summary>
/// Обработчик <see cref="SeedMarketsCommand"/>. Для каждого элемента seed-списка проверяет,
/// не отслеживается ли рынок уже (по PolymarketConditionId), и добавляет только новые
/// </summary>
/// <param name="marketRepository">Репозиторий рынков</param>
/// <param name="unitOfWork">Единица работы для сохранения изменений</param>
public sealed class SeedMarketsCommandHandler(ITrackedMarketRepository marketRepository, IUnitOfWork unitOfWork) : IRequestHandler<SeedMarketsCommand, int>
{
    /// <inheritdoc />
    public async Task<int> Handle(SeedMarketsCommand request, CancellationToken ct)
    {
        var conditionIds = MarketSeedList.Items.Select(x => x.ConditionId).ToList();
        var existingConditionIds = await marketRepository.GetExistingConditionIdsAsync(conditionIds, ct);

        var newMarkets = MarketSeedList.Items.Where(seedItem => !existingConditionIds.Contains(seedItem.ConditionId))
            .Select(seedItem => new TrackedMarket
            {
                Id = Guid.NewGuid(),
                PolymarketConditionId = seedItem.ConditionId,
                Question = seedItem.Question,
                AssetSymbol = seedItem.AssetSymbol,
                CoinGeckoAssetId = seedItem.CoinGeckoAssetId,
                ThresholdValue = seedItem.ThresholdValue,
                Direction = seedItem.Direction,
                ResolutionDate = seedItem.ResolutionDate,
                Status = MarketStatus.Open,
                CreatedAt = DateTimeOffset.UtcNow
            })
            .ToList();

        foreach (var market in newMarkets)
            marketRepository.Add(market);

        if (newMarkets.Count > 0)
            await unitOfWork.SaveChangesAsync(ct);

        return newMarkets.Count;
    }
}