using PolymarketPredictor.Application.Common.Models.Polymarket;

namespace PolymarketPredictor.Application.Common.Interfaces;

/// <summary>
/// Клиент публичного Polymarket Gamma API (https://gamma-api.polymarket.com), без авторизации
/// </summary>
public interface IPolymarketClient
{
    /// <summary>
    /// Забрать открытые события по тегу категории (например, "crypto") постранично.
    /// Возвращает сырые данные события/рынка Gamma API как есть — парсинг вопроса
    /// и извлечение порога делается выше, в Application
    /// </summary>
    /// <param name="tag">Слаг тега категории Gamma API</param>
    /// <param name="limit">Максимальное количество событий в ответе</param>
    /// <param name="ct">Токен отмены</param>
    Task<IReadOnlyList<GammaEventDto>> GetOpenEventsByTagAsync(string tag, int limit, CancellationToken ct);

    /// <summary>
    /// Забрать актуальную карточку одного рынка по conditionId (цена, объём, ликвидность, статус)
    /// </summary>
    /// <param name="conditionId">conditionId рынка в Polymarket</param>
    /// <param name="ct">Токен отмены</param>
    Task<GammaMarketDto?> GetMarketByConditionIdAsync(string conditionId, CancellationToken ct);
}