using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using PolymarketPredictor.Application.Common.Interfaces;
using PolymarketPredictor.Application.Common.Models.Polymarket;

namespace PolymarketPredictor.Infrastructure.ExternalClients.Polymarket;

/// <summary>
/// Клиент публичного Polymarket Gamma API (https://gamma-api.polymarket.com), без авторизации
/// </summary>
/// <param name="httpClient">HTTP-клиент, сконфигурированный с BaseAddress и политиками устойчивости</param>
public sealed class PolymarketClient(HttpClient httpClient) : IPolymarketClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <inheritdoc />
    public async Task<IReadOnlyList<GammaEventDto>> GetOpenEventsByTagAsync(string tag, int limit, CancellationToken ct)
    {
        var url = $"events?tag_slug={Uri.EscapeDataString(tag)}&active=true&closed=false&limit={limit}";
        var events = await httpClient.GetFromJsonAsync<List<GammaEventApiModel>>(url, JsonOptions, ct) ?? [];

        return [.. events.Select(MapEvent)];
    }

    /// <inheritdoc />
    public async Task<GammaMarketDto?> GetMarketByConditionIdAsync(string conditionId, CancellationToken ct)
    {
        var url = $"markets?condition_ids={Uri.EscapeDataString(conditionId)}";
        var markets = await httpClient.GetFromJsonAsync<List<GammaMarketApiModel>>(url, JsonOptions, ct) ?? [];

        var market = markets.FirstOrDefault();
        return market is null ? null : MapMarket(market);
    }

    /// <summary>
    /// Преобразует модель события Gamma API во внутренний DTO Application-слоя
    /// </summary>
    /// <param name="e">Модель события, полученная из API</param>
    private static GammaEventDto MapEvent(GammaEventApiModel e) =>
        new(e.Slug, e.Title, [.. e.Markets.Select(MapMarket)]);

    /// <summary>
    /// Преобразует модель рынка Gamma API во внутренний DTO Application-слоя
    /// </summary>
    /// <param name="m">Модель рынка, полученная из API</param>
    private static GammaMarketDto MapMarket(GammaMarketApiModel m) =>
        new(ConditionId: m.ConditionId, Question: m.Question, Active: m.Active, Closed: m.Closed,
            Volume24hr: m.Volume24hr ?? 0m, Liquidity: m.Liquidity ?? 0m, YesPrice: ParseYesPrice(m.OutcomePrices));

    /// <summary>
    /// Парсит implied-вероятность исхода "Yes" из строки-JSON outcomePrices
    /// </summary>
    /// <param name="outcomePricesJson">Значение поля outcomePrices как есть из ответа API (JSON внутри строки)</param>
    /// <returns>Вероятность 0..1, либо 0, если поле пустое или отсутствует</returns>
    private static double ParseYesPrice(string? outcomePricesJson)
    {
        if (string.IsNullOrWhiteSpace(outcomePricesJson))
            return 0d;

        var prices = JsonSerializer.Deserialize<List<string>>(outcomePricesJson, JsonOptions);

        if (prices is null || prices.Count == 0)
            return 0d;

        return double.Parse(prices[0], CultureInfo.InvariantCulture);
    }
}