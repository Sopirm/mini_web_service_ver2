namespace Pr2.ModulesAndDi.Core;

/// <summary>
/// Интерфейс для работы со складом.
/// </summary>
public interface IStockRepository
{
    Task AddStockItemAsync(StockItem item);
    Task<StockItem?> GetStockItemByPartIdAsync(Guid partId);
    Task<IReadOnlyList<StockItem>> GetAllStockItemsAsync();
    Task UpdateStockItemAsync(StockItem item);
    Task RemoveStockItemAsync(Guid partId);
    Task IncreaseStockAsync(Guid partId, int quantity);
    Task DecreaseStockAsync(Guid partId, int quantity);
}
