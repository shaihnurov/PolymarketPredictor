using System.Net;
using System.Text;
using FluentAssertions;
using PolymarketPredictor.Infrastructure.ExternalClients.Polymarket;
using PolymarketPredictor.UnitTests.TestUtils;

namespace PolymarketPredictor.UnitTests.ExternalClients;

/// <summary>
/// Юнит-тесты <see cref="PolymarketClient"/> на самописном HTTP-обработчике
/// </summary>
public class PolymarketClientTests
{
    /// <summary>
    /// Проверяет, что implied-вероятность корректно парсится из строки-JSON outcomePrices
    /// </summary>
    [Fact]
    public async Task GetMarketByConditionIdAsync_ParsesYesPriceFromOutcomePricesJson()
    {
        const string json = """
        [
          {
            "conditionId": "0xabc",
            "question": "Will BTC be above $150,000 on Dec 31, 2026?",
            "active": true,
            "closed": false,
            "outcomes": "[\"Yes\", \"No\"]",
            "outcomePrices": "[\"0.245\", \"0.755\"]",
            "volume24hr": 40000,
            "liquidity": 60000
          }
        ]
        """;

        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://gamma-api.polymarket.com/") };
        var sut = new PolymarketClient(httpClient);

        var result = await sut.GetMarketByConditionIdAsync("0xabc", CancellationToken.None);

        result.Should().NotBeNull();
        result!.YesPrice.Should().Be(0.245d);
        result.Volume24hr.Should().Be(40000m);
        result.Liquidity.Should().Be(60000m);
    }

    /// <summary>
    /// Проверяет, что при пустом ответе API метод возвращает null, а не бросает исключение
    /// </summary>
    [Fact]
    public async Task GetMarketByConditionIdAsync_ReturnsNull_WhenNoMarketsFound()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("[]", Encoding.UTF8, "application/json")
        });
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://gamma-api.polymarket.com/") };
        var sut = new PolymarketClient(httpClient);

        var result = await sut.GetMarketByConditionIdAsync("missing", CancellationToken.None);

        result.Should().BeNull();
    }
}