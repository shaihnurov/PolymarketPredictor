namespace PolymarketPredictor.Application.Common.Models.Polymarket;

/// <summary>
/// Минимальный набор полей рынка Gamma API, нужный системе
/// </summary>
/// <param name="ConditionId">Уникальный идентификатор рынка в Polymarket</param>
/// <param name="Question">Текст вопроса рынка</param>
/// <param name="Active">Рынок торгуется в данный момент</param>
/// <param name="Closed">Рынок уже закрыт (резолвлен)</param>
/// <param name="Volume24hr">Объём торгов за последние 24 часа, USD</param>
/// <param name="Liquidity">Текущая ликвидность рынка, USD</param>
/// <param name="YesPrice">Implied-вероятность исхода "Yes" — первый элемент outcomePrices, уже распарсенный в double</param>
public sealed record GammaMarketDto(string ConditionId, string Question, bool Active, bool Closed, decimal Volume24hr, decimal Liquidity, double YesPrice);