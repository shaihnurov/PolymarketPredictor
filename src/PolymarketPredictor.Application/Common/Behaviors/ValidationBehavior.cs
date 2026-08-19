using FluentValidation;
using MediatR;

namespace PolymarketPredictor.Application.Common.Behaviors;

/// <summary>
/// Поведение для валидации входящих запросов перед их обработкой. Используется в конвейере MediatR
/// </summary>
/// <typeparam name="TRequest">Тип запроса</typeparam>
/// <typeparam name="TResponse">Тип ответа</typeparam>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    /// <summary>
    /// Проверяет запрос перед передачей его следующему обработчику
    /// </summary>
    /// <param name="request">Входящий запрос</param>
    /// <param name="next">Следующий обработчик в конвейере</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Ответ от следующего обработчика</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!validators.Any())
            return await next(ct);

        var context = new ValidationContext<TRequest>(request);

        var failures = validators.Select(v => v.Validate(context)).SelectMany(e => e.Errors).Where(f => f != null).ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next(ct);
    }
}