namespace PolymarketPredictor.Application.Common.Interfaces;

/// <summary>
/// Фиксирует все изменения, накопленные через репозитории, одной транзакцией
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Сохранить все накопленные изменения в БД
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Количество затронутых строк</returns>
    Task<int> SaveChangesAsync(CancellationToken ct);
}