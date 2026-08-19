using PolymarketPredictor.Application.Common.Models.CoinGecko;

namespace PolymarketPredictor.Application.Common.Interfaces;

/// <summary>
/// Клиент публичного CoinGecko API (https://api.coingecko.com/api/v3), без ключа, с учётом rate limit
/// </summary>
public interface ICoinGeckoClient
{
    /// <summary>
    /// Текущая спот-цена актива в USD
    /// </summary>
    /// <param name="coinGeckoAssetId">Идентификатор актива в CoinGecko, например "bitcoin"</param>
    /// <param name="ct">Токен отмены</param>
    Task<decimal> GetCurrentPriceAsync(string coinGeckoAssetId, CancellationToken ct);

    /// <summary>
    /// Дневная история цен закрытия за последние дни — нужна для расчёта исторической волатильности
    /// (<see cref="Domain.Entities.NormalizedIndicator.DailyVolatility30d"/>)
    /// </summary>
    /// <param name="coinGeckoAssetId">Идентификатор актива в CoinGecko</param>
    /// <param name="days">Глубина истории в днях</param>
    /// <param name="ct">Токен отмены</param>
    Task<IReadOnlyList<DailyPricePoint>> GetDailyPriceHistoryAsync(string coinGeckoAssetId, int days, CancellationToken ct);
}