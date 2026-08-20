using System.Text.RegularExpressions;
using PolymarketPredictor.Domain.Enums;

namespace PolymarketPredictor.Application.Markets.Parsing;

/// <summary>
/// Опциональный парсер текста вопроса Polymarket в структурированные поля (актив, порог,
/// направление). Не используется как основной путь наполнения БД в MVP — реальные вопросы
/// Polymarket неоднородны по формулировкам ("above"/"reach"/"hit", "$150k"/"$150,000"),
/// поэтому основа MVP ручной <see cref="Seed.MarketSeedList"/>. Этот парсер задел на
/// будущее расширение списка рынков без ручной разметки каждого нового вопроса
/// </summary>
public static partial class MarketQuestionParser
{
    [GeneratedRegex(@"Will\s+(?<asset>[A-Za-z]+)\s+(?:reach|be above|go above|hit)\s+\$(?<threshold>[\d,]+(?:\.\d+)?)", RegexOptions.IgnoreCase)]
    private static partial Regex AboveThresholdRegex();

    [GeneratedRegex(@"Will\s+(?<asset>[A-Za-z]+)\s+(?:drop below|fall below|be below|go below)\s+\$(?<threshold>[\d,]+(?:\.\d+)?)", RegexOptions.IgnoreCase)]
    private static partial Regex BelowThresholdRegex();

    /// <summary>
    /// Пытается распознать актив, порог и направление из текста вопроса
    /// </summary>
    /// <param name="question">Текст вопроса рынка, как на Polymarket</param>
    /// <returns>Результат разбора, либо null, если формулировка не распознана ни одним из шаблонов</returns>
    public static ParsedQuestion? TryParse(string question)
    {
        var aboveMatch = AboveThresholdRegex().Match(question);

        if (aboveMatch.Success)
            return BuildResult(aboveMatch, ThresholdDirection.Above);

        var belowMatch = BelowThresholdRegex().Match(question);

        if (belowMatch.Success)
            return BuildResult(belowMatch, ThresholdDirection.Below);

        return null;
    }

    /// <summary>
    /// Собирает результат разбора из совпавшей regex-группы
    /// </summary>
    /// <param name="match">Совпадение одного из шаблонов</param>
    /// <param name="direction">Направление, соответствующее сработавшему шаблону</param>
    private static ParsedQuestion BuildResult(Match match, ThresholdDirection direction)
    {
        var asset = match.Groups["asset"].Value.ToUpperInvariant();
        var thresholdText = match.Groups["threshold"].Value.Replace(",", "");
        var threshold = decimal.Parse(thresholdText, System.Globalization.CultureInfo.InvariantCulture);

        return new ParsedQuestion(asset, threshold, direction);
    }
}

/// <summary>
/// Результат разбора текста вопроса рынка
/// </summary>
/// <param name="AssetSymbol">Распознанный тикер актива</param>
/// <param name="ThresholdValue">Распознанное пороговое значение</param>
/// <param name="Direction">Распознанное направление условия</param>
public sealed record ParsedQuestion(string AssetSymbol, decimal ThresholdValue, ThresholdDirection Direction);