using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PolymarketPredictor.Application.Markets.Dtos;
using PolymarketPredictor.Persistence;

namespace PolymarketPredictor.IntegrationTests;

/// <summary>
/// Сквозные тесты эндпоинтов рынков поверх реального PostgreSQL в Docker-контейнере.
/// Проверяют не бизнес-формулы (это уже покрыто юнит-тестами), а то, что миграции,
/// DI-граф и весь HTTP-конвейер работают вместе как единое целое
/// </summary>
public sealed class MarketsEndpointsTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    /// <summary>
    /// Применяет миграции к контейнерной БД перед каждым тестом
    /// </summary>
    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    /// <summary>
    /// Очищает данные между тестами, чтобы они не влияли друг на друга (миграции переприменять не нужно)
    /// </summary>
    public async Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE predictions, normalized_indicators, raw_snapshots, tracked_markets CASCADE;");
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMarketList_ReturnsEmptyList_WhenDatabaseIsEmpty()
    {
        var response = await _client.GetAsync("/api/markets");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var markets = await response.Content.ReadFromJsonAsync<List<MarketListItemDto>>();
        markets.Should().BeEmpty();
    }

    [Fact]
    public async Task SeedThenList_ReturnsAllSeededMarkets_WithoutPredictionsYet()
    {
        var seedResponse = await _client.PostAsync("/api/markets/seed", content: null);
        seedResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var listResponse = await _client.GetAsync("/api/markets");
        var markets = await listResponse.Content.ReadFromJsonAsync<List<MarketListItemDto>>();

        markets.Should().NotBeNull();
        markets!.Should().HaveCount(5);
        markets.Should().OnlyContain(m => m.LatestPredictedProbability == null);
    }

    [Fact]
    public async Task Seed_IsIdempotent_WhenCalledTwice()
    {
        await _client.PostAsync("/api/markets/seed", content: null);
        var secondSeedResponse = await _client.PostAsync("/api/markets/seed", content: null);

        var secondResult = await secondSeedResponse.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        secondResult!["addedCount"].Should().Be(0);
    }

    [Fact]
    public async Task GetMarketDetail_ReturnsNotFound_ForUnknownId()
    {
        var response = await _client.GetAsync($"/api/markets/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAccuracyStats_ReturnsNullScore_WhenNoResolvedMarkets()
    {
        var response = await _client.GetAsync("/api/stats/accuracy");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var stats = await response.Content.ReadFromJsonAsync<AccuracyStatsDto>();
        stats!.BrierScore.Should().BeNull();
    }
}