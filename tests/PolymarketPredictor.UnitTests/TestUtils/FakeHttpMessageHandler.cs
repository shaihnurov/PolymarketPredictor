namespace PolymarketPredictor.UnitTests.TestUtils;

/// <summary>
/// Тестовый HttpMessageHandler
/// </summary>
/// <param name="responder">Функция, которая по входящему запросу возвращает нужный ответ</param>
public sealed class FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
{
    /// <summary>
    /// Возвращает ответ, сконструированный делегатом <see cref="responder"/>, вместо реального HTTP-запроса
    /// </summary>
    /// <param name="request">Исходящий HTTP-запрос</param>
    /// <param name="cancellationToken">Токен отмены</param>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(responder(request));
}