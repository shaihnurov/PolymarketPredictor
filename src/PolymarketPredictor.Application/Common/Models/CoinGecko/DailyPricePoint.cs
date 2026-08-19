namespace PolymarketPredictor.Application.Common.Models.CoinGecko;

/// <summary>
/// Одна дневная точка ценовой истории актива
/// </summary>
/// <param name="Date">Дата (UTC), к которой относится цена закрытия</param>
/// <param name="ClosePrice">Цена закрытия на эту дату, USD</param>
public sealed record DailyPricePoint(DateOnly Date, decimal ClosePrice);