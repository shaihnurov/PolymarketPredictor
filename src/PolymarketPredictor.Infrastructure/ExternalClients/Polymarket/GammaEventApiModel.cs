using PolymarketPredictor.Infrastructure.ExternalClients.Polymarket;

/// <summary>
/// Модель события Gamma API как есть в JSON-ответе. Поля минимальны — только то, что реально
/// используется. Перед доработкой сверяться с docs.polymarket.com: точный список полей,
/// имена query-параметров и лимиты периодически меняются
/// </summary>
internal sealed class GammaEventApiModel
{
    /// <summary>
    /// Уникальный слаг события в Polymarket
    /// /summary>
    public string Slug { get; set; } = default!;

    /// <summary>
    /// Заголовок события
    /// </summary>
    public string Title { get; set; } = default!;

    /// <summary>
    /// Рынки, входящие в это событие
    /// </summary>
    public List<GammaMarketApiModel> Markets { get; set; } = [];
}