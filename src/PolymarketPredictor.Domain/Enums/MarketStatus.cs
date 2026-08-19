namespace PolymarketPredictor.Domain.Enums;

/// <summary>
/// Статус жизненного цикла отслеживаемого рынка
/// </summary>
public enum MarketStatus
{
    /// <summary>
    /// Рынок ещё открыт, резолюция не наступила
    /// </summary>
    Open,
    /// <summary>
    /// Рынок закрыт, есть фактический исход (<see cref="Entities.TrackedMarket.ActualOutcome"/>)
    /// </summary>
    Resolved
}