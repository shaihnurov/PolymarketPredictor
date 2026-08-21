using MediatR;
using Microsoft.AspNetCore.Mvc;
using PolymarketPredictor.Application.Markets.Commands;
using PolymarketPredictor.Application.Markets.Dtos;
using PolymarketPredictor.Application.Markets.Queries;

namespace PolymarketPredictor.WebApi.Controllers;

/// <summary>
/// Эндпоинты по отслеживаемым рынкам и их прогнозам
/// </summary>
/// <param name="sender">MediatR-отправитель запросов и команд</param>
[ApiController]
[Route("api/markets")]
public sealed class MarketsController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Список всех отслеживаемых рынков с последним прогнозом по каждому
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<MarketListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MarketListItemDto>>> GetList(CancellationToken ct)
    {
        var result = await sender.Send(new GetMarketListQuery(), ct);
        return Ok(result);
    }

    /// <summary>
    /// Детальная карточка одного рынка с последним прогнозом
    /// </summary>
    /// <param name="id">Идентификатор рынка</param>
    /// <param name="ct">Токен отмены</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MarketDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MarketDetailDto>> GetDetail(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetMarketDetailQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Полная история прогнозов рынка, от старых к новым — для графика
    /// </summary>
    /// <param name="id">Идентификатор рынка</param>
    /// <param name="ct">Токен отмены</param>
    [HttpGet("{id:guid}/history")]
    [ProducesResponseType(typeof(List<PredictionHistoryItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PredictionHistoryItemDto>>> GetHistory(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetMarketHistoryQuery(id), ct);
        return Ok(result);
    }

    /// <summary>
    /// Ручной триггер полного цикла синка по всем открытым рынкам — та же команда, что
    /// использует фоновая служба по расписанию. Удобно для демо, без ожидания интервала таймера
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    [HttpPost("sync")]
    [ProducesResponseType(typeof(SyncAllMarketsResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<SyncAllMarketsResult>> Sync(CancellationToken ct)
    {
        var result = await sender.Send(new SyncAllOpenMarketsCommand(), ct);
        return Ok(result);
    }

    /// <summary>
    /// Наполняет БД рынками из ручного seed-списка. Идемпотентно — повторный вызов не создаёт дубликатов
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    [HttpPost("seed")]
    public async Task<IActionResult> Seed(CancellationToken ct)
    {
        var addedCount = await sender.Send(new SeedMarketsCommand(), ct);
        return Ok(new { addedCount });
    }
}