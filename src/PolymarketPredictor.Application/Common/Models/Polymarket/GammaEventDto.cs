namespace PolymarketPredictor.Application.Common.Models.Polymarket;

/// <summary>
/// Минимальный набор полей события Gamma API, нужный системе (не весь ответ API)
/// </summary>
/// <param name="Slug">Уникальный слаг события</param>
/// <param name="Title">Заголовок события</param>
/// <param name="Markets">Рынки, входящие в это событие</param>
public sealed record GammaEventDto(string Slug, string Title, IReadOnlyList<GammaMarketDto> Markets);