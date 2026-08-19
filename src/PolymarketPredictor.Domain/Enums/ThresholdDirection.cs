namespace PolymarketPredictor.Domain.Enums;

/// <summary>
/// Направление порогового условия рынка: цена актива должна оказаться
/// выше или ниже <see cref="Entities.TrackedMarket.ThresholdValue"/> к дате резолюции
/// </summary>
public enum ThresholdDirection
{
    /// <summary>
    /// Условие сбывается, если цена актива окажется выше порога
    /// </summary>
    Above,

    /// <summary>
    /// Условие сбывается, если цена актива окажется ниже порога
    /// </summary>
    Below
}