using MediatR;
using Microsoft.AspNetCore.Mvc;
using PolymarketPredictor.Application.Markets.Dtos;
using PolymarketPredictor.Application.Markets.Queries;

namespace PolymarketPredictor.WebApi.Controllers;

/// <summary>
/// Эндпоинты статистики и метрик калибровки модели
/// </summary>
/// <param name="sender">MediatR-отправитель запросов и команд</param>
[ApiController]
[Route("api/stats")]
public sealed class StatsController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Метрика калибровки модели (Brier score) по всем резолвленным рынкам
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    [HttpGet("accuracy")]
    [ProducesResponseType(typeof(AccuracyStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AccuracyStatsDto>> GetAccuracy(CancellationToken ct)
    {
        var result = await sender.Send(new GetAccuracyStatsQuery(), ct);
        return Ok(result);
    }
}