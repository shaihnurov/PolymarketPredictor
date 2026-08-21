namespace PolymarketPredictor.Application.Markets.Dtos;

/// <summary>
/// Метрика калибровки модели (Brier score) — то, как система понимает, что ошиблась
/// Считается по последнему прогнозу каждого уже резолвленного рынка: среднее
/// ((predictedProbability − actualOutcome)²). Меньше — лучше; 0 — идеальная калибровка,
/// 0.25 — уровень случайного угадывания при p=0.5
/// </summary>
/// <param name="ResolvedMarketsCount">Сколько рынков всего уже резолвлено</param>
/// <param name="MarketsIncludedInScore">Сколько из резолвленных рынков реально попало в расчёт (у них есть хотя бы один прогноз)</param>
/// <param name="BrierScore">Сам Brier score, либо null, если резолвленных рынков с прогнозами ещё нет</param>
public sealed record AccuracyStatsDto(int ResolvedMarketsCount, int MarketsIncludedInScore, double? BrierScore);