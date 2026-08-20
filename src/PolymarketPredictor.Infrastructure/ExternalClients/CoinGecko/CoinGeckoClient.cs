using System.Net.Http.Json;
using System.Text.Json.Serialization;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Application.Common.Models.CoinGecko;

namespace PolymarketPredictor.Infrastructure.ExternalClients.CoinGecko;

/// <summary>
/// Клиент публичного CoinGecko API (https://api.coingecko.com/api/v3), без ключа
/// Free-tier rate limit жёсткий — вызывающий код (фоновый синк) обязан не дёргать чаще, чем раз в цикл синка на актив
/// </summary>
/// <param name="httpClient">HTTP-клиент, сконфигурированный с BaseAddress и политиками устойчивости</param>
public sealed class CoinGeckoClient(HttpClient httpClient) : ICoinGeckoClient
{
    /// <inheritdoc />
    public async Task<decimal> GetCurrentPriceAsync(string coinGeckoAssetId, CancellationToken ct)
    {
        var url = $"simple/price?ids={Uri.EscapeDataString(coinGeckoAssetId)}&vs_currencies=usd";
        var response = await httpClient.GetFromJsonAsync<Dictionary<string, Dictionary<string, decimal>>>(url, ct);

        if (response is null || !response.TryGetValue(coinGeckoAssetId, out var byCurrency) || !byCurrency.TryGetValue("usd", out var price))
            throw new InvalidOperationException($"CoinGecko не вернул цену для '{coinGeckoAssetId}'");

        return price;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DailyPricePoint>> GetDailyPriceHistoryAsync(string coinGeckoAssetId, int days, CancellationToken ct)
    {
        var url = $"coins/{Uri.EscapeDataString(coinGeckoAssetId)}/market_chart?vs_currency=usd&days={days}&interval=daily";
        var response = await httpClient.GetFromJsonAsync<MarketChartApiModel>(url, ct)
            ?? throw new InvalidOperationException($"CoinGecko не вернул историю цен для '{coinGeckoAssetId}'");

        return [.. response.Prices.Select(p 
            => new DailyPricePoint(DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeMilliseconds((long)p[0]).UtcDateTime), (decimal)p[1]))];
    }

    /// <summary>
    /// Модель ответа эндпоинта market_chart CoinGecko как есть в JSON
    /// </summary>
    private sealed class MarketChartApiModel
    {
        /// <summary>
        /// Массив точек цены вида [unixTimestampMs, price]
        /// </summary>
        [JsonPropertyName("prices")]
        public List<double[]> Prices { get; set; } = [];
    }
}