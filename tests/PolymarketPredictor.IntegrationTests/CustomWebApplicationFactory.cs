using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;

namespace PolymarketPredictor.IntegrationTests;

/// <summary>
/// Фабрика тестового хоста приложения. Поднимает реальный PostgreSQL в Docker-контейнере
/// и подменяет строку подключения из appsettings.json
/// </summary>
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:16")
        .WithDatabase("polymarket_predictor_test").WithUsername("postgres").WithPassword("postgres").Build();

    /// <summary>
    /// Запускает контейнер PostgreSQL перед первым использованием фабрики в тестах
    /// </summary>
    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
    }

    /// <summary>
    /// Останавливает и удаляет контейнер PostgreSQL после завершения всех тестов в классе
    /// </summary>
    public new async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Postgres"] = _postgresContainer.GetConnectionString(),
                ["MarketSync:Enabled"] = "false"
            });
        });
    }
}