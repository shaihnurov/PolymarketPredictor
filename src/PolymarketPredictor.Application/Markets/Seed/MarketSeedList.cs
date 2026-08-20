using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.Application.Markets.Seed;

/// <summary>
/// Ручной seed-список реальных пороговых крипто-рынков Polymarket для MVP
/// ConditionId — заглушки, перед демо нужно подставить реальные conditionId, найденные
/// на gamma-api.polymarket.com или на сайте Polymarket (раздел Crypto) — они меняются
/// по мере появления/резолюции рынков, актуальный список нельзя зашить намертво навсегда
/// </summary>
public static class MarketSeedList
{
    /// <summary>
    /// Список рынков для первоначального наполнения БД командой SeedMarketsCommand 
    /// </summary>
    public static readonly IReadOnlyList<MarketSeedItem> Items =
    [
        new(
            ConditionId: "0x02deb9538f5c123373adaa4ee6217b01745f1662bc902e46ac92f3fe6f8741e8",
            Question: "Will Bitcoin hit $150k by December 31, 2026?",
            AssetSymbol: "BTC",
            CoinGeckoAssetId: "bitcoin",
            ThresholdValue: 150000m,
            Direction: ThresholdDirection.Above,
            ResolutionDate: new DateOnly(2026, 12, 31)),

        new(
            ConditionId: "0x11f8f845d4965a4a7418c57272c92ba23242f12faef555bc79eeebf5416eeea1",
            Question: "Will XRP reach $5.00 by December 31, 2026?",
            AssetSymbol: "XRP",
            CoinGeckoAssetId: "ripple",
            ThresholdValue: 5m,
            Direction: ThresholdDirection.Above,
            ResolutionDate: new DateOnly(2026, 12, 31)),

        new(
            ConditionId: "0xca7f2d347ea03f04f5e6f8859491716111352a1bdc086cb2ee95df766779ec95",
            Question: "Will XRP reach $4.20 by December 31, 2026?",
            AssetSymbol: "XRP",
            CoinGeckoAssetId: "ripple",
            ThresholdValue: 4.20m,
            Direction: ThresholdDirection.Above,
            ResolutionDate: new DateOnly(2026, 12, 31)),

        new(
            ConditionId: "0x1c4fd67ab2a67f508672a69153559911244048b79a40cbe341d12f985ba90a13",
            Question: "Will Ethereum reach $5,000 by December 31, 2026?",
            AssetSymbol: "ETH",
            CoinGeckoAssetId: "ethereum",
            ThresholdValue: 5000m,
            Direction: ThresholdDirection.Above,
            ResolutionDate: new DateOnly(2026, 12, 31)),

        new(
            ConditionId: "0xa54d855c4c25e2e48687c90efc4bfd49c50dbedd2be984d29784a47b2ff81bdc",
            Question: "Will Solana reach $400 by December 31, 2026?",
            AssetSymbol: "SOL",
            CoinGeckoAssetId: "solana",
            ThresholdValue: 400m,
            Direction: ThresholdDirection.Above,
            ResolutionDate: new DateOnly(2026, 12, 31)),
    ];
}