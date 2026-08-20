using System.Net;
using System.Text;
using FluentAssertions;
using PolymarketPredictor.Infrastructure.ExternalClients.CoinGecko;
using PolymarketPredictor.UnitTests.TestUtils;

namespace PolymarketPredictor.UnitTests.ExternalClients;

/// <summary>
/// Юнит-тесты <see cref="CoinGeckoClient"/> на самописном HTTP-обработчике
/// </summary>
public class CoinGeckoClientTests
{
    /// <summary>
    /// Проверяет, что текущая цена корректно парсится из ответа /simple/price
    /// </summary>
    [Fact]
    public async Task GetCurrentPriceAsync_ParsesUsdPrice()
    {
        const string json = """{"bitcoin":{"usd":118000.5}}""";

        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });

        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.coingecko.com/api/v3/") };
        var sut = new CoinGeckoClient(httpClient);

        var price = await sut.GetCurrentPriceAsync("bitcoin", CancellationToken.None);

        price.Should().Be(118000.5m);
    }

    /// <summary>
    /// Проверяет, что дневная история цен корректно парсится из ответа /market_chart
    /// </summary>
    [Fact]
    public async Task GetDailyPriceHistoryAsync_ParsesPricesArray()
    {
        const string json = """
        {
          "prices": [
            [1735689600000, 100000.0],
            [1735776000000, 101000.0]
          ]
        }
        """;

        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });

        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.coingecko.com/api/v3/") };
        var sut = new CoinGeckoClient(httpClient);

        var history = await sut.GetDailyPriceHistoryAsync("bitcoin", 30, CancellationToken.None);

        history.Should().HaveCount(2);
        history[0].ClosePrice.Should().Be(100000.0m);
        history[1].ClosePrice.Should().Be(101000.0m);
    }
}