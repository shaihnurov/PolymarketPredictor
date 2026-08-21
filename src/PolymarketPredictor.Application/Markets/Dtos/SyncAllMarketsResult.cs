namespace PolymarketPredictor.Application.Markets.Dtos;

/// <summary>
/// Итог одного цикла синка по всем открытым рынкам
/// </summary>
/// <param name="MarketsProcessed">Сколько рынков успешно синхронизировано</param>
/// <param name="MarketsFailed">Сколько рынков завершились ошибкой (см. логи за подробностями)</param>
public sealed record SyncAllMarketsResult(int MarketsProcessed, int MarketsFailed);